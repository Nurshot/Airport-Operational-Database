namespace AODB.Domain.Models;

public class AuthenticationResult
{
    public bool IsSuccess { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public string? UserId { get; init; }
    public string? Username { get; init; }
    public IList<string> Roles { get; init; } = new List<string>();
    public string? ErrorMessage { get; init; }
    
    public static AuthenticationResult Success(
        string accessToken, 
        string refreshToken, 
        DateTime expiresAt,
        string userId,
        string username,
        IList<string> roles)
    {
        return new AuthenticationResult
        {
            IsSuccess = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            UserId = userId,
            Username = username,
            Roles = roles
        };
    }
    
    public static AuthenticationResult Failure(string errorMessage)
    {
        return new AuthenticationResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

