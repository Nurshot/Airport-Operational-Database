using AODB.Application.Authentication.Commands.Login;
using AODB.Application.Authentication.Commands.Logout;
using AODB.Application.Authentication.Commands.RefreshToken;
using AODB.Application.Common.Interfaces;
using AODB.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace AODB.Web.Endpoints;

public class Authentication : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(Login, "login")
            .MapPost(RefreshToken, "refresh")
            .MapPost(Logout, "logout")
            .MapGet(ValidateTokenAsync, "validate");
    }

    public async Task<IResult> Login(ISender sender, LoginCommand command, ILoggingService loggingService)
    {
        var requestId = Guid.NewGuid().ToString();
        loggingService.LogAuthenticationAttempt(command.Username, requestId);

        try
        {
            var result = await sender.Send(command);

           
            if (!result.IsSuccess)
            {
                loggingService.LogAuthenticationFailure(command.Username, requestId, result.ErrorMessage ?? "authfail");
                return Results.BadRequest(new { error = result.ErrorMessage });
            }

            loggingService.LogAuthenticationSuccess(command.Username, result.UserId??"null", requestId, result.Roles);
            Console.Write(result);
            return Results.Ok(new
            {
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken,
                expiresAt = result.ExpiresAt,
                user = new
                {
                    id = result.UserId,
                    username = result.Username,
                    roles = result.Roles
                }
            });
        }
        catch (Exception ex)
        {
            loggingService.LogAuthenticationError(command.Username, requestId, ex);
            throw;
        }
    }

    public async Task<IResult> RefreshToken(ISender sender, RefreshTokenCommand command, ILoggingService loggingService)
    {
        var requestId = Guid.NewGuid().ToString();
        loggingService.LogRefreshTokenAttempt(requestId);

        try
        {
            var result = await sender.Send(command);
            
            if (!result.IsSuccess)
            {
                
                return Results.BadRequest(new { error = result.ErrorMessage });
            }

            loggingService.LogRefreshTokenSuccess(requestId);

            return Results.Ok(new
            {
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken,
                expiresAt = result.ExpiresAt
            });
        }
        catch (Exception ex)
        {
            loggingService.LogRefreshTokenError(requestId, ex);
            throw;
        }
    }

    public async Task<IResult> Logout(ISender sender, LogoutCommand command, ILoggingService loggingService)
    {
        var requestId = Guid.NewGuid().ToString();
        loggingService.LogLogoutAttempt(requestId);

        try
        {
            var result = await sender.Send(command);
            
            if (!result)
            {
                loggingService.LogLogoutFailure(requestId);
                return Results.BadRequest(new { error = "Logout failed" });
            }

            loggingService.LogLogoutSuccess(requestId);

            return Results.Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            loggingService.LogLogoutError(requestId, ex);
            throw;
        }
    }

    public async Task<IResult> ValidateTokenAsync(IServiceProvider serviceProvider, [FromHeader(Name = "Authorization")] string? authorization, ILoggingService loggingService)
    {
        var requestId = Guid.NewGuid().ToString();
        loggingService.LogTokenValidationAttempt(requestId, !string.IsNullOrEmpty(authorization));

        try
        {
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
            {
                loggingService.LogTokenValidationFailure(requestId, "InvalidAuthorizationHeader");
                return Results.BadRequest(new { error = "Invalid authorization header" });
            }

            var token = authorization.Substring("Bearer ".Length);
            
            // IAuthenticationService üzerinden token validation yapalım
            var authService = serviceProvider.GetRequiredService<IAuthenticationService>();
            var isValid = await authService.ValidateTokenAsync(token);
            
            if (!isValid)
            {
                loggingService.LogTokenValidationFailure(requestId, "TokenValidationFailed");
                return Results.Unauthorized();
            }
            
            loggingService.LogTokenValidationSuccess(requestId);
            
            return Results.Ok(new { valid = true, message = "Token is valid" });
        }
        catch (Exception ex)
        {
            loggingService.LogTokenValidationError(requestId, ex);
            throw;
        }
    }
}
