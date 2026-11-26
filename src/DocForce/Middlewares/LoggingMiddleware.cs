using System.Diagnostics;

namespace DocForce.API.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;

        _logger.LogInformation("Incomming request: {method} {path}", request.Method, request.Path);

        var sw = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            _logger.LogInformation("Request: {method} {path} processed in: {elapsedMs}",
                request.Method, request.Path, sw.ElapsedMilliseconds);
        }
    }
}
