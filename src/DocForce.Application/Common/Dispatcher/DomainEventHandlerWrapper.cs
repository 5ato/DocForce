using DocForce.Application.Common.Dispatcher.Interfaces;
using DocForce.Domain.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DocForce.Application.Common.Dispatcher;

internal abstract class DomainEventHandlerBase
{
    public abstract ValueTask Handle(
        IDomainEvent domainEvent,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default);
}

internal class DomainEventHandlerWrapper<TDomainEvent> : DomainEventHandlerBase
    where TDomainEvent : IDomainEvent
{
    public async override ValueTask Handle(
        IDomainEvent domainEvent, 
        IServiceProvider serviceProvider, 
        CancellationToken cancellationToken = default)
    {
        var typed = (TDomainEvent)domainEvent;
        var domainEventHandlers = serviceProvider.GetServices<IDomainEventHandler<TDomainEvent>>();

        foreach (var domainEventHandler in domainEventHandlers)
            await domainEventHandler.HandleAsync(typed, cancellationToken);
    }
}
