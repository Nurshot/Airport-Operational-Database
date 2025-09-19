using AODB.Domain.Models;

namespace AODB.Application.Common.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResult> LoginAsync(string username, string password);
    Task<AuthenticationResult> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync(string accessToken);
    Task<bool> ValidateTokenAsync(string accessToken);
}

