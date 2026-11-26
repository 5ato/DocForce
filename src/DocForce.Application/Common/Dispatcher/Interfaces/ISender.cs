namespace DocForce.Application.Common.Dispatcher.Interfaces;

public interface ISender
{
    ValueTask<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default);
}
