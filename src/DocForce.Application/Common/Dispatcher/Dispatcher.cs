using DocForce.Application.Common.Dispatcher.Interfaces;
using DocForce.Domain.Common.Interfaces;
using System.Collections.Frozen;

namespace DocForce.Application.Common.Dispatcher;

internal sealed class Dispatcher : ISender, IPublisher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DispatcherRegistry _registry;

    public Dispatcher(IServiceProvider serviceProvider, DispatcherRegistry registry)
    {
        _serviceProvider = serviceProvider;
        _registry = registry;
    }

    public ValueTask Publish<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default) 
        where TDomainEvent : IDomainEvent
    {
        if (!_registry.DomainEventWrappers.TryGetValue(domainEvent.GetType(), out var wrapper))
        {
            throw new InvalidOperationException(
                $"No handler registered for domain event type '{domainEvent.GetType().FullName}'.");
        }
        return wrapper.Handle(domainEvent, _serviceProvider, cancellationToken);
    }

    public ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (!_registry.RequestWrappers.TryGetValue(request.GetType(), out var wrapper))
        {
            throw new InvalidOperationException(
                $"No handler registered for request type '{request.GetType().FullName}'.");
        }

        return ((RequestHandlerBase<TResponse>)wrapper).HandleAsync(request, _serviceProvider, cancellationToken);
    }
}

internal sealed class DispatcherRegistry
{
    public FrozenDictionary<Type, RequestHandlerBase> RequestWrappers { get; }
    public FrozenDictionary<Type, DomainEventHandlerBase> DomainEventWrappers { get; }

    public DispatcherRegistry(
        FrozenDictionary<Type, RequestHandlerBase> requestWrappers,
        FrozenDictionary<Type, DomainEventHandlerBase> domainEventWrappers)
    {
        RequestWrappers = requestWrappers;
        DomainEventWrappers = domainEventWrappers;
    }
}
