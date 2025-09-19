using AODB.Application.Common.Interfaces;
using Serilog;

namespace AODB.Infrastructure.Logging;

public class LoggingService : ILoggingService
{
    private static readonly Serilog.ILogger Logger = Log.ForContext<LoggingService>();

    public void LogAuthenticationAttempt(string username, string requestId)
    {
        Logger.ForContext("EventType", "AuthenticationLogin")
              .ForContext("RequestId", requestId)
              .ForContext("Username", username)
              .Information("Login attempt: {Username} [{RequestId}]", username, requestId);
    }

    public void LogAuthenticationSuccess(string username, string userId, string requestId, IList<string> roles)
    {
        Logger.ForContext("EventType", "AuthenticationLoginSuccess")
              .ForContext("RequestId", requestId)
              .ForContext("Username", username)
              .ForContext("UserId", userId)
              .ForContext("Roles", roles)
              .Information("Login success: {Username} -> {UserId} [{RequestId}]", username, userId, requestId);
    }

    public void LogSend(string username,string content)
    {
        Logger.ForContext("EventType", "API-Call")
                .ForContext("username", username)
                .ForContext("content", content)
              .Information("API-Call {username}  Content: {content}", username, content);
    }

    public void LogSendFail(string username, string content)
    {
        Logger.ForContext("EventType", "Exception API-Call")
                .ForContext("username", username)
                .ForContext("content", content)
              .Information("Exception {username}  Content: {content}", username, content);


        
    }


    public void LogAuthenticationFailure(string username, string requestId, string errorMessage)
    {
        Logger.ForContext("EventType", "AuthenticationLoginFailed")
              .ForContext("RequestId", requestId)
              .ForContext("Username", username)
              .ForContext("ErrorMessage", errorMessage)
              .Warning("Login failed: {Username} - {ErrorMessage} [{RequestId}]", username, errorMessage, requestId);
    }

    public void LogAuthenticationError(string username, string requestId, Exception exception)
    {
        Logger.ForContext("EventType", "AuthenticationLoginError")
              .ForContext("RequestId", requestId)
              .ForContext("Username", username)
              .ForContext("ExceptionType", exception.GetType().Name)
              .Error(exception, "Login error: {Username} [{RequestId}]", username, requestId);
    }

    public void LogRefreshTokenAttempt(string requestId)
    {
        Logger.ForContext("EventType", "AuthenticationRefreshToken")
              .ForContext("RequestId", requestId)
              .Information("Refresh token attempt [{RequestId}]", requestId);
    }

    public void LogRefreshTokenSuccess(string requestId)
    {
        Logger.ForContext("EventType", "AuthenticationRefreshTokenSuccess")
              .ForContext("RequestId", requestId)
              .Information("Refresh token success [{RequestId}]", requestId);
    }

    public void LogRefreshTokenFailure(string requestId, string errorMessage)
    {
        Logger.ForContext("EventType", "AuthenticationRefreshTokenFailed")
              .ForContext("RequestId", requestId)
              .ForContext("ErrorMessage", errorMessage)
              .Warning("Refresh token failed: {ErrorMessage} [{RequestId}]", errorMessage, requestId);
    }

    public void LogRefreshTokenError(string requestId, Exception exception)
    {
        Logger.ForContext("EventType", "AuthenticationRefreshTokenError")
              .ForContext("RequestId", requestId)
              .ForContext("ExceptionType", exception.GetType().Name)
              .Error(exception, "Refresh token error [{RequestId}]", requestId);
    }

    public void LogLogoutAttempt(string requestId)
    {
        Logger.ForContext("EventType", "AuthenticationLogout")
              .ForContext("RequestId", requestId)
              .Information("Logout attempt [{RequestId}]", requestId);
    }

    public void LogLogoutSuccess(string requestId)
    {
        Logger.ForContext("EventType", "AuthenticationLogoutSuccess")
              .ForContext("RequestId", requestId)
              .Information("Logout success [{RequestId}]", requestId);
    }

    public void LogLogoutFailure(string requestId)
    {
        Logger.ForContext("EventType", "AuthenticationLogoutFailed")
              .ForContext("RequestId", requestId)
              .Warning("Logout failed [{RequestId}]", requestId);
    }

    public void LogLogoutError(string requestId, Exception exception)
    {
        Logger.ForContext("EventType", "AuthenticationLogoutError")
              .ForContext("RequestId", requestId)
              .ForContext("ExceptionType", exception.GetType().Name)
              .Error(exception, "Logout error [{RequestId}]", requestId);
    }

    public void LogTokenValidationAttempt(string requestId, bool hasAuthHeader)
    {
        Logger.ForContext("EventType", "AuthenticationValidateToken")
              .ForContext("RequestId", requestId)
              .ForContext("HasAuthorizationHeader", hasAuthHeader)
              .Information("Token validation attempt [{RequestId}]", requestId);
    }

    public void LogTokenValidationSuccess(string requestId)
    {
        Logger.ForContext("EventType", "AuthenticationValidateTokenSuccess")
              .ForContext("RequestId", requestId)
              .Information("Token validation success [{RequestId}]", requestId);
    }

    public void LogTokenValidationFailure(string requestId, string reason)
    {
        Logger.ForContext("EventType", "AuthenticationValidateTokenFailed")
              .ForContext("RequestId", requestId)
              .ForContext("ErrorReason", reason)
              .Warning("Token validation failed: {Reason} [{RequestId}]", reason, requestId);
    }

    public void LogTokenValidationError(string requestId, Exception exception)
    {
        Logger.ForContext("EventType", "AuthenticationValidateTokenError")
              .ForContext("RequestId", requestId)
              .ForContext("ExceptionType", exception.GetType().Name)
              .Error(exception, "Token validation error [{RequestId}]", requestId);
    }

    public void LogKeycloakRequest(string operation, string username, string requestId, string endpoint)
    {
        Logger.ForContext("EventType", $"Keycloak{operation}")
              .ForContext("RequestId", requestId)
              .ForContext("Username", username)
              .ForContext("Endpoint", endpoint)
              .Information("Keycloak {Operation}: {Username} -> {Endpoint} [{RequestId}]", operation, username, endpoint, requestId);
    }

    public void LogKeycloakResponse(string operation, string requestId, int statusCode, bool isSuccess)
    {
        var level = isSuccess ? Serilog.Events.LogEventLevel.Information : Serilog.Events.LogEventLevel.Warning;
        Logger.ForContext("EventType", $"Keycloak{operation}Response")
              .ForContext("RequestId", requestId)
              .ForContext("StatusCode", statusCode)
              .ForContext("IsSuccess", isSuccess)
              .Write(level, "Keycloak {Operation} response: {StatusCode} [{RequestId}]", operation, statusCode, requestId);
    }

    public void LogKeycloakError(string operation, string requestId, Exception exception)
    {
        Logger.ForContext("EventType", $"Keycloak{operation}Error")
              .ForContext("RequestId", requestId)
              .ForContext("ExceptionType", exception.GetType().Name)
              .Error(exception, "Keycloak {Operation} error [{RequestId}]", operation, requestId);
    }
}
