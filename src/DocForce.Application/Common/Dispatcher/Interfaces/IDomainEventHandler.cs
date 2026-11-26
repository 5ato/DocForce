using DocForce.Domain.Common.Interfaces;

namespace DocForce.Application.Common.Dispatcher.Interfaces;

public interface IDomainEventHandler { }

public interface IDomainEventHandler<in TDomainEvent> : IDomainEventHandler 
    where TDomainEvent : IDomainEvent
{
    ValueTask HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
