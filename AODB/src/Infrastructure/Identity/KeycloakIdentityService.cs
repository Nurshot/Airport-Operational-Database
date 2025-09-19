using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AODB.Application.Common.Interfaces;
using AODB.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AODB.Infrastructure.Identity;

public class KeycloakIdentityService : IIdentityService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private string? _cachedAdminToken;
    private DateTime _tokenExpiration = DateTime.MinValue;

    public KeycloakIdentityService(
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var response = await SendAdminRequest<object>(HttpMethod.Get, $"/users/{userId}", null);
        if (response.IsSuccessStatusCode)
        {
            var user = await response.Content.ReadFromJsonAsync<KeycloakUser>();
            return user?.Username;
        }
        return null;
    }

    public async Task<string?> GetUserIdByNameAsync(string username)
    {
        var response = await SendAdminRequest<object>(HttpMethod.Get, $"/users?username={username}", null);
        if (response.IsSuccessStatusCode)
        {
            var users = await response.Content.ReadFromJsonAsync<List<KeycloakUser>>();
            return users?.FirstOrDefault()?.Id;
        }
        return null;
    }

    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        var response = await SendAdminRequest<object>(HttpMethod.Get, $"/users/{userId}/role-mappings/realm", null);
        if (response.IsSuccessStatusCode)
        {
            var roles = await response.Content.ReadFromJsonAsync<List<KeycloakRole>>();
            return roles?.Select(r => r.Name).ToList() ?? new List<string>();
        }
        return new List<string>();
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var roles = await GetUserRolesAsync(userId);
        return roles.Contains(role);
    }

    public Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        // Basit yetkilendirme kontrolü
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true) return Task.FromResult(false);

        // Policy bazlı yetkilendirme için özel kontroller eklenebilir
        return Task.FromResult(true);
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password, IList<string>? roles = null)
    {
        var user = new KeycloakUser
        {
            Username = userName,
            Enabled = true,
            Credentials = new[]
            {
                new KeycloakCredential
                {
                    Type = "password",
                    Value = password,
                    Temporary = false
                }
            }
        };

        var response = await SendAdminRequest(HttpMethod.Post, "/users", user);
        if (!response.IsSuccessStatusCode)
        {
            return (Result.Failure(new[] { "Failed to create user" }), string.Empty);
        }

        var userId = response.Headers.Location?.Segments.Last();
        if (string.IsNullOrEmpty(userId))
        {
            return (Result.Failure(new[] { "Failed to get created user ID" }), string.Empty);
        }

        if (roles?.Any() == true)
        {
            foreach (var role in roles)
            {
                await AssignRoleAsync(userId, role);
            }
        }

        return (Result.Success(), userId);
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var response = await SendAdminRequest<object>(HttpMethod.Delete, $"/users/{userId}", null);
        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(new[] { "Failed to delete user" });
    }

    public async Task<Result> AssignRoleAsync(string userId, string role)
    {
        // Önce rolü al
        var roleResponse = await SendAdminRequest<object>(HttpMethod.Get, $"/roles/{role}", null);
        if (!roleResponse.IsSuccessStatusCode)
        {
            return Result.Failure(new[] { $"Role {role} not found" });
        }

        var roleInfo = await roleResponse.Content.ReadFromJsonAsync<KeycloakRole>();
        var roleRepresentation = new[] { roleInfo };

        var response = await SendAdminRequest(
            HttpMethod.Post,
            $"/users/{userId}/role-mappings/realm",
            roleRepresentation);

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(new[] { $"Failed to assign role {role}" });
    }

    public async Task<Result> RemoveRoleAsync(string userId, string role)
    {
        var roleResponse = await SendAdminRequest<object>(HttpMethod.Get, $"/roles/{role}", null);
        if (!roleResponse.IsSuccessStatusCode)
        {
            return Result.Failure(new[] { $"Role {role} not found" });
        }

        var roleInfo = await roleResponse.Content.ReadFromJsonAsync<KeycloakRole>();
        var roleRepresentation = new[] { roleInfo };

        var response = await SendAdminRequest(
            HttpMethod.Delete,
            $"/users/{userId}/role-mappings/realm",
            roleRepresentation);

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(new[] { $"Failed to remove role {role}" });
    }

    public async Task<Result> UpdateUserAsync(string userId, string? username = null, string? email = null)
    {
        var user = new KeycloakUser
        {
            Username = username,
            Email = email
        };

        var response = await SendAdminRequest(HttpMethod.Put, $"/users/{userId}", user);
        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(new[] { "Failed to update user" });
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var authority = _configuration["Keycloak:Authority"] ?? throw new InvalidOperationException("Keycloak:Authority configuration is missing");
            var realm = _configuration["Keycloak:Realm"] ?? throw new InvalidOperationException("Keycloak:Realm configuration is missing");
            
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"{authority}/realms/{realm}/protocol/openid-connect/userinfo");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task<string> GetAdminToken()
    {
        if (_cachedAdminToken != null && DateTime.UtcNow < _tokenExpiration)
        {
            return _cachedAdminToken;
        }

        var adminClientId = _configuration["Keycloak:AdminClientId"] ?? throw new InvalidOperationException("Keycloak:AdminClientId configuration is missing");
        var adminClientSecret = _configuration["Keycloak:AdminClientSecret"] ?? throw new InvalidOperationException("Keycloak:AdminClientSecret configuration is missing");
        
        var tokenRequest = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = adminClientId,
            ["client_secret"] = adminClientSecret
        };

        var authority = _configuration["Keycloak:Authority"] ?? throw new InvalidOperationException("Keycloak:Authority configuration is missing");
        
        var response = await _httpClient.PostAsync(
            $"{authority}/realms/master/protocol/openid-connect/token",
            new FormUrlEncodedContent(tokenRequest));

        if (response.IsSuccessStatusCode)
        {
            var tokenResponse = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>();
            _cachedAdminToken = tokenResponse?.Access_Token;
            _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse?.Expires_In ?? 60);
            return _cachedAdminToken ?? string.Empty;
        }

        return string.Empty;
    }

    private async Task<HttpResponseMessage> SendAdminRequest<TContent>(HttpMethod method, string path, TContent? content = default)
    {
        var token = await GetAdminToken();
        var authority = _configuration["Keycloak:Authority"] ?? throw new InvalidOperationException("Keycloak:Authority configuration is missing");
        var realm = _configuration["Keycloak:Realm"] ?? throw new InvalidOperationException("Keycloak:Realm configuration is missing");
        
        var request = new HttpRequestMessage(method, $"{authority}/admin/realms/{realm}{path}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (content != null)
        {
            request.Content = new StringContent(
                JsonSerializer.Serialize(content),
                System.Text.Encoding.UTF8,
                "application/json");
        }

        return await _httpClient.SendAsync(request);
    }
}

public class KeycloakUser
{
    public string? Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public bool Enabled { get; set; }
    public KeycloakCredential[]? Credentials { get; set; }
}

public class KeycloakCredential
{
    public string Type { get; set; } = "password";
    public string Value { get; set; } = string.Empty;
    public bool Temporary { get; set; }
}

public class KeycloakRole
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class KeycloakTokenResponse
{
    public string Access_Token { get; set; } = string.Empty;
    public int Expires_In { get; set; }
    public string Token_Type { get; set; } = string.Empty;
}