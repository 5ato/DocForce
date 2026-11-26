using DocForce.Application.Common.Dispatcher.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DocForce.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _log;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> log)
    {
        _log = log;
    }

    public async ValueTask<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken = default)
    {
        var requestName = typeof(TRequest).Name;
        _log.LogInformation("Handling {RequestName}", requestName);

        var sw = Stopwatch.StartNew();
        try
        {
            var response = await next();
            sw.Stop();
            _log.LogInformation("Handled {RequestName} in {Elapsed}ms", requestName, sw.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _log.LogError(ex, "Handler {RequestName} threw after {Elapsed}ms", requestName, sw.ElapsedMilliseconds);
            throw;
        }
    }
}
