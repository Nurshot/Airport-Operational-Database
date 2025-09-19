using AODB.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AODB.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger;
    private readonly IUser _currentUser;

    public LoggingBehaviour(ILogger<TRequest> logger, IUser currentUser)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUser.Id ?? "System";
        var userName = _currentUser.Username ?? "Unknown";

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // Structured logging with Serilog context
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["EventType"] = "MediatRRequestStarted",
            ["RequestName"] = requestName,
            ["UserId"] = userId,
            ["UserName"] = userName,
            ["RequestId"] = Guid.NewGuid().ToString()
        }))
        {
            _logger.LogInformation("AODB Request Started: {RequestName} by {UserId} ({UserName})",
                requestName, userId, userName);

            try 
            {
                var response = await next();
                
                stopwatch.Stop();
                
                _logger.LogInformation("AODB Request Completed: {RequestName} by {UserId} in {ElapsedMs}ms",
                    requestName, userId, stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                _logger.LogError(ex, "AODB Request Failed: {RequestName} by {UserId} after {ElapsedMs}ms",
                    requestName, userId, stopwatch.ElapsedMilliseconds);
                
                throw;
            }
        }
    }
}

