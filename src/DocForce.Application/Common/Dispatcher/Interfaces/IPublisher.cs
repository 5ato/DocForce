using DocForce.Domain.Common.Interfaces;

namespace DocForce.Application.Common.Dispatcher.Interfaces;

public interface IPublisher
{
    ValueTask Publish<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
        where TDomainEvent : IDomainEvent;
}
