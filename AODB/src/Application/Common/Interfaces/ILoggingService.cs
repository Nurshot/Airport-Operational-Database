namespace AODB.Application.Common.Interfaces;

public interface ILoggingService
{
    void LogAuthenticationAttempt(string username, string requestId);
    void LogAuthenticationSuccess(string username, string userId, string requestId, IList<string> roles);
    void LogAuthenticationFailure(string username, string requestId, string errorMessage);
    void LogAuthenticationError(string username, string requestId, Exception exception);
    void LogSend(string username, string content);
    void LogSendFail(string username, string content);
    
    void LogRefreshTokenAttempt(string requestId);
    void LogRefreshTokenSuccess(string requestId);
    void LogRefreshTokenFailure(string requestId, string errorMessage);
    void LogRefreshTokenError(string requestId, Exception exception);
    
    void LogLogoutAttempt(string requestId);
    void LogLogoutSuccess(string requestId);
    void LogLogoutFailure(string requestId);
    void LogLogoutError(string requestId, Exception exception);
    
    void LogTokenValidationAttempt(string requestId, bool hasAuthHeader);
    void LogTokenValidationSuccess(string requestId);
    void LogTokenValidationFailure(string requestId, string reason);
    void LogTokenValidationError(string requestId, Exception exception);
    
    // Keycloak service logging
    void LogKeycloakRequest(string operation, string username, string requestId, string endpoint);
    void LogKeycloakResponse(string operation, string requestId, int statusCode, bool isSuccess);
    void LogKeycloakError(string operation, string requestId, Exception exception);
}
