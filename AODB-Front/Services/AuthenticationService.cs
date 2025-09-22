using System.Text;
using Newtonsoft.Json;
using AODB_Front.Models;
using AODB_Front.Config;

namespace AODB_Front.Services
{
    public class AuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public AuthenticationService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(AppConfig.TimeoutSeconds);
            _baseUrl = AppConfig.AuthenticationEndpoint;
        }

        public async Task<AuthenticationResult> LoginAsync(string username, string password)
        {
            try
            {
                var loginRequest = new LoginRequest
                {
                    Username = username,
                    Password = password
                };

                var json = JsonConvert.SerializeObject(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);
                if (response.IsSuccessStatusCode)
                {
                   

                    var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
                    

                    return new AuthenticationResult
                    {
                        IsSuccess = true,
                        AccessToken = loginResponse?.AccessToken,
                        RefreshToken = loginResponse?.RefreshToken,
                        ExpiresAt = loginResponse?.ExpiresAt,
                        User = loginResponse?.User
                    };
                }
                else
                {
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    return new AuthenticationResult
                    {
                        IsSuccess = false,
                        ErrorMessage = errorResponse?.error?.ToString() ?? "Login Failed."
                    };
                }
            }
            catch (Exception ex)
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{_baseUrl}/validate");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
