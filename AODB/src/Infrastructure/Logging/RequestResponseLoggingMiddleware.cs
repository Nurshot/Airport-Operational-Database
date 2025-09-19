using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace AODB.Infrastructure.Logging;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Serilog.ILogger _logger;
    private readonly ILogger<RequestResponseLoggingMiddleware> _microsoftLogger;

    public RequestResponseLoggingMiddleware(
        RequestDelegate next, 
        Serilog.ILogger logger,
        ILogger<RequestResponseLoggingMiddleware> microsoftLogger)
    {
        _next = next;
        _logger = logger;
        _microsoftLogger = microsoftLogger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        // Request bilgilerini topla
        var requestInfo = await CaptureRequestInfo(context);
        
        // Unique request ID oluştur
        var requestId = Guid.NewGuid().ToString();
        context.Items["RequestId"] = requestId;

        // Request başlangıcını logla
        LogRequestStarted(requestInfo, requestId);

        // Response'u yakalamak için stream'i wrap et
        var originalResponseBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        try
        {
            await _next(context);
            stopwatch.Stop();

            // Response bilgilerini topla
            var responseInfo = await CaptureResponseInfo(context, responseBodyStream);
            
            // Request tamamlandığını logla
            LogRequestCompleted(requestInfo, responseInfo, requestId, stopwatch.Elapsed);

            // Original response body'yi geri kopyala
            await responseBodyStream.CopyToAsync(originalResponseBodyStream);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            // Hata durumunu logla
            LogRequestFailed(requestInfo, ex, requestId, stopwatch.Elapsed);
            
            throw;
        }
        finally
        {
            context.Response.Body = originalResponseBodyStream;
        }
    }

    private async Task<RequestInfo> CaptureRequestInfo(HttpContext context)
    {
        var request = context.Request;
        
        // Request body'yi oku (eğer varsa)
        string? requestBody = null;
        if (request.ContentLength > 0 && 
            request.ContentType?.Contains("application/json") == true)
        {
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            request.Body.Position = 0;
        }

        return new RequestInfo
        {
            Method = request.Method,
            Path = request.Path,
            QueryString = request.QueryString.ToString(),
            Headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
            Body = requestBody,
            UserAgent = request.Headers.UserAgent.ToString(),
            RemoteIpAddress = context.Connection.RemoteIpAddress?.ToString(),
            UserId = context.User?.Identity?.Name ?? "Anonymous",
            ContentType = request.ContentType,
            ContentLength = request.ContentLength
        };
    }

    private async Task<ResponseInfo> CaptureResponseInfo(HttpContext context, MemoryStream responseBodyStream)
    {
        var response = context.Response;
        
        // Response body'yi oku
        string? responseBody = null;
        if (responseBodyStream.Length > 0)
        {
            responseBodyStream.Position = 0;
            using var reader = new StreamReader(responseBodyStream, Encoding.UTF8, leaveOpen: true);
            responseBody = await reader.ReadToEndAsync();
            responseBodyStream.Position = 0;
        }

        return new ResponseInfo
        {
            StatusCode = response.StatusCode,
            Headers = response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
            Body = responseBody,
            ContentType = response.ContentType,
            ContentLength = response.ContentLength
        };
    }

    private void LogRequestStarted(RequestInfo requestInfo, string requestId)
    {
        _logger.ForContext("EventType", "HttpRequestStarted")
              .ForContext("RequestId", requestId)
              .ForContext("Method", requestInfo.Method)
              .ForContext("Path", requestInfo.Path)
              .ForContext("QueryString", requestInfo.QueryString)
              .ForContext("UserId", requestInfo.UserId)
              .ForContext("UserAgent", requestInfo.UserAgent)
              .ForContext("RemoteIpAddress", requestInfo.RemoteIpAddress)
              .ForContext("ContentType", requestInfo.ContentType)
              .ForContext("ContentLength", requestInfo.ContentLength)
              .ForContext("RequestHeaders", requestInfo.Headers, true)
              .ForContext("RequestBody", requestInfo.Body, true)
              .ForContext("Timestamp", DateTimeOffset.UtcNow)
              .Information("HTTP Request Started: {Method} {Path}", requestInfo.Method, requestInfo.Path);
    }

    private void LogRequestCompleted(RequestInfo requestInfo, ResponseInfo responseInfo, string requestId, TimeSpan elapsed)
    {
        var logLevel = responseInfo.StatusCode >= 400 ? Serilog.Events.LogEventLevel.Warning : Serilog.Events.LogEventLevel.Information;
        
        _logger.ForContext("EventType", "HttpRequestCompleted")
              .ForContext("RequestId", requestId)
              .ForContext("Method", requestInfo.Method)
              .ForContext("Path", requestInfo.Path)
              .ForContext("UserId", requestInfo.UserId)
              .ForContext("StatusCode", responseInfo.StatusCode)
              .ForContext("ElapsedMs", elapsed.TotalMilliseconds)
              .ForContext("ResponseHeaders", responseInfo.Headers, true)
              .ForContext("ResponseBody", responseInfo.Body, true)
              .ForContext("ResponseContentType", responseInfo.ContentType)
              .ForContext("ResponseContentLength", responseInfo.ContentLength)
              .ForContext("Timestamp", DateTimeOffset.UtcNow)
              .Write(logLevel, "HTTP Request Completed: {Method} {Path} responded {StatusCode} in {ElapsedMs}ms", 
                     requestInfo.Method, requestInfo.Path, responseInfo.StatusCode, elapsed.TotalMilliseconds);
    }

    private void LogRequestFailed(RequestInfo requestInfo, Exception exception, string requestId, TimeSpan elapsed)
    {
        _logger.ForContext("EventType", "HttpRequestFailed")
              .ForContext("RequestId", requestId)
              .ForContext("Method", requestInfo.Method)
              .ForContext("Path", requestInfo.Path)
              .ForContext("UserId", requestInfo.UserId)
              .ForContext("ElapsedMs", elapsed.TotalMilliseconds)
              .ForContext("ErrorMessage", exception.Message)
              .ForContext("StackTrace", exception.StackTrace)
              .ForContext("ExceptionType", exception.GetType().Name)
              .ForContext("Timestamp", DateTimeOffset.UtcNow)
              .Error(exception, "HTTP Request Failed: {Method} {Path} failed in {ElapsedMs}ms", 
                     requestInfo.Method, requestInfo.Path, elapsed.TotalMilliseconds);
    }
}

public record RequestInfo
{
    public string Method { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public string? QueryString { get; init; }
    public Dictionary<string, string> Headers { get; init; } = new();
    public string? Body { get; init; }
    public string? UserAgent { get; init; }
    public string? RemoteIpAddress { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string? ContentType { get; init; }
    public long? ContentLength { get; init; }
}

public record ResponseInfo
{
    public int StatusCode { get; init; }
    public Dictionary<string, string> Headers { get; init; } = new();
    public string? Body { get; init; }
    public string? ContentType { get; init; }
    public long? ContentLength { get; init; }
}

