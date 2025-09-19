using System.Diagnostics;
using AODB.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace AODB.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public PerformanceBehaviour(
        ILogger<TRequest> logger,
        IUser user,
        IIdentityService identityService)
    {
        _timer = new Stopwatch();
        _logger = logger;
        _user = user;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();
        var requestName = typeof(TRequest).Name;
        var userId = _user.Id ?? "System";

        try
        {
            var response = await next();
            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            // Performance warning for slow requests
            if (elapsedMilliseconds > 500)
            {
                var userName = string.Empty;
                if (!string.IsNullOrEmpty(userId) && userId != "System")
                {
                    userName = await _identityService.GetUserNameAsync(userId) ?? "Unknown";
                }

                // Structured logging with performance context
                using (_logger.BeginScope(new Dictionary<string, object>
                {
                    ["EventType"] = "PerformanceWarning",
                    ["RequestName"] = requestName,
                    ["UserId"] = userId,
                    ["UserName"] = userName,
                    ["ElapsedMilliseconds"] = elapsedMilliseconds,
                    ["PerformanceThreshold"] = 500
                }))
                {
                    _logger.LogWarning("AODB Long Running Request: {RequestName} ({ElapsedMilliseconds} milliseconds) by {UserId} ({UserName})",
                        requestName, elapsedMilliseconds, userId, userName);
                }
            }

            return response;
        }
        catch (Exception ex)
        {
            _timer.Stop();
            
            // Log failed request with performance info
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["EventType"] = "PerformanceError",
                ["RequestName"] = requestName,
                ["UserId"] = userId,
                ["ElapsedMilliseconds"] = _timer.ElapsedMilliseconds
            }))
            {
                _logger.LogError(ex, "AODB Request Failed: {RequestName} by {UserId} after {ElapsedMilliseconds}ms",
                    requestName, userId, _timer.ElapsedMilliseconds);
            }
            
            throw;
        }
    }
}
