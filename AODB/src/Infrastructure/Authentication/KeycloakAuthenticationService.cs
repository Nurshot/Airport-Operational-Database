using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AODB.Application.Common.Interfaces;
using AODB.Domain.Models;
using Microsoft.Extensions.Configuration;

namespace AODB.Infrastructure.Authentication;

public class KeycloakAuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILoggingService _loggingService;

    public KeycloakAuthenticationService(HttpClient httpClient, IConfiguration configuration, ILoggingService loggingService)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _loggingService = loggingService;
    }

    public async Task<AuthenticationResult> LoginAsync(string username, string password)
    {
        var requestId = Guid.NewGuid().ToString();

        try
        {
            var authority = GetConfigValue("Keycloak:Authority");
            var realm = GetConfigValue("Keycloak:Realm");
            var clientId = GetConfigValue("Keycloak:ClientId");
            var clientSecret = GetConfigValue("Keycloak:ClientSecret");

            var tokenRequest = new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["username"] = username,
                ["password"] = password,
                ["scope"] = "openid profile email"
            };

            var tokenUrl = $"{authority}/realms/{realm}/protocol/openid-connect/token";
            
            _loggingService.LogKeycloakRequest("Login", username, requestId, tokenUrl);

            var response = await _httpClient.PostAsync(tokenUrl, new FormUrlEncodedContent(tokenRequest));

            _loggingService.LogKeycloakResponse("Login", requestId, (int)response.StatusCode, response.IsSuccessStatusCode);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return AuthenticationResult.Failure($"Login failed: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            if (tokenResponse == null)
            {
                return AuthenticationResult.Failure($"Invalid token response. Raw response: {responseContent}");
            }

            if (string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                return AuthenticationResult.Failure($"Empty access token received. Raw response: {responseContent}");
            }

            // Token'dan kullanıcı bilgilerini al
            var userInfo = await GetUserInfoAsync(tokenResponse.AccessToken);
            
            return AuthenticationResult.Success(
                tokenResponse.AccessToken,
                tokenResponse.RefreshToken ?? string.Empty,
                DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                userInfo.Sub ?? string.Empty,
                userInfo.PreferredUsername ?? username,
                userInfo.Roles ?? new List<string>()
            );
        }
        catch (Exception ex)
        {
            _loggingService.LogKeycloakError("Login", requestId, ex);
            return AuthenticationResult.Failure($"Login error: {ex.Message}");
        }
    }

    public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken)
    {
        var requestId = Guid.NewGuid().ToString();
        
        try
        {
            var authority = GetConfigValue("Keycloak:Authority");
            var realm = GetConfigValue("Keycloak:Realm");
            var clientId = GetConfigValue("Keycloak:ClientId");
            var clientSecret = GetConfigValue("Keycloak:ClientSecret");

            var tokenRequest = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["refresh_token"] = refreshToken
            };

            var tokenUrl = $"{authority}/realms/{realm}/protocol/openid-connect/token";
            _loggingService.LogKeycloakRequest("RefreshToken", "Anonymous", requestId, tokenUrl);

            var response = await _httpClient.PostAsync(tokenUrl, new FormUrlEncodedContent(tokenRequest));

            _loggingService.LogKeycloakResponse("RefreshToken", requestId, (int)response.StatusCode, response.IsSuccessStatusCode);

            if (!response.IsSuccessStatusCode)
            {
                return AuthenticationResult.Failure("Refresh token failed");
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>();
            if (tokenResponse == null)
            {
                return AuthenticationResult.Failure("Invalid token response");
            }

            var userInfo = await GetUserInfoAsync(tokenResponse.AccessToken);

            return AuthenticationResult.Success(
                tokenResponse.AccessToken,
                tokenResponse.RefreshToken ?? refreshToken,
                DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                userInfo.Sub ?? string.Empty,
                userInfo.PreferredUsername ?? string.Empty,
                userInfo.Roles ?? new List<string>()
            );
        }
        catch (Exception ex)
        {
            _loggingService.LogKeycloakError("RefreshToken", requestId, ex);
            return AuthenticationResult.Failure($"Refresh token error: {ex.Message}");
        }
    }

    public async Task<bool> LogoutAsync(string accessToken)
    {
        var requestId = Guid.NewGuid().ToString();
        
        try
        {
            var authority = GetConfigValue("Keycloak:Authority");
            var realm = GetConfigValue("Keycloak:Realm");
            var clientId = GetConfigValue("Keycloak:ClientId");
            var clientSecret = GetConfigValue("Keycloak:ClientSecret");

            var logoutRequest = new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["token"] = accessToken
            };

            var logoutUrl = $"{authority}/realms/{realm}/protocol/openid-connect/logout";
            _loggingService.LogKeycloakRequest("Logout", "Anonymous", requestId, logoutUrl);

            var response = await _httpClient.PostAsync(logoutUrl, new FormUrlEncodedContent(logoutRequest));

            _loggingService.LogKeycloakResponse("Logout", requestId, (int)response.StatusCode, response.IsSuccessStatusCode);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _loggingService.LogKeycloakError("Logout", requestId, ex);
            return false;
        }
    }

    public async Task<bool> ValidateTokenAsync(string accessToken)
    {
        var requestId = Guid.NewGuid().ToString();
        
        try
        {
            var authority = GetConfigValue("Keycloak:Authority");
            var realm = GetConfigValue("Keycloak:Realm");
            var userInfoUrl = $"{authority}/realms/{realm}/protocol/openid-connect/userinfo";
            
            _loggingService.LogKeycloakRequest("ValidateToken", "Anonymous", requestId, userInfoUrl);
            
            var userInfo = await GetUserInfoAsync(accessToken);
            var isValid = !string.IsNullOrEmpty(userInfo.Sub);
            
            _loggingService.LogKeycloakResponse("ValidateToken", requestId, isValid ? 200 : 401, isValid);
            
            return isValid;
        }
        catch (Exception ex)
        {
            _loggingService.LogKeycloakError("ValidateToken", requestId, ex);
            return false;
        }
    }

    private async Task<KeycloakUserInfo> GetUserInfoAsync(string accessToken)
    {
        var authority = GetConfigValue("Keycloak:Authority");
        var realm = GetConfigValue("Keycloak:Realm");

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{authority}/realms/{realm}/protocol/openid-connect/userinfo");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<KeycloakUserInfo>() ?? new KeycloakUserInfo();
        }

        return new KeycloakUserInfo();
    }

    private string GetConfigValue(string key)
    {
        return _configuration[key] ?? throw new InvalidOperationException($"{key} configuration is missing");
    }
}

public class KeycloakTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
    
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
}

public class KeycloakUserInfo
{
    [JsonPropertyName("sub")]
    public string? Sub { get; set; }
    
    [JsonPropertyName("preferred_username")]
    public string? PreferredUsername { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("realm_access")]
    public RealmAccess? RealmAccess { get; set; }
    
    public IList<string> Roles => RealmAccess?.Roles ?? new List<string>();
}

public class RealmAccess
{
    [JsonPropertyName("roles")]
    public IList<string> Roles { get; set; } = new List<string>();
}
