using DocForce.Application.Common.Dispatcher.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DocForce.Application.Common.Dispatcher;

internal abstract class RequestHandlerBase;

internal abstract class RequestHandlerBase<TResponse> : RequestHandlerBase
{
    public abstract ValueTask<TResponse> HandleAsync(
        IRequest<TResponse> request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default);
}

internal sealed class RequestHandlerWrapper<TRequest, TResponse> : RequestHandlerBase<TResponse>
    where TRequest : IRequest<TResponse>
{
    public override ValueTask<TResponse> HandleAsync(
        IRequest<TResponse> request, 
        IServiceProvider serviceProvider, 
        CancellationToken cancellationToken = default)
    {
        var typed = (TRequest)request;
        var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>();

        RequestHandlerDelegate<TResponse> pipeline = () => handler.Handle(typed, cancellationToken);

        foreach (var behavior in behaviors)
        {
            var next = pipeline;
            var current = behavior;
            pipeline = () => current.Handle(typed, next, cancellationToken);
        }

        return pipeline();
    }
}
