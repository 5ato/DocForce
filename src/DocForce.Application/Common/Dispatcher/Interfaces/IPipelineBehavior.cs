namespace DocForce.Application.Common.Dispatcher.Interfaces;

public delegate ValueTask<TResponse> RequestHandlerDelegate<TResponse>();

public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    ValueTask<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default);
}
