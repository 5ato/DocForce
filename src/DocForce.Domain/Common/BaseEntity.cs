using DocForce.Domain.Common.Interfaces;

namespace DocForce.Domain.Common;

public abstract class BaseEntity : IEntity
{
    private readonly IList<IDomainEvent> _domainEvents = [];

    public Guid Id { get; set; }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void RemoveDomainEvent(IDomainEvent domainEvent) => _domainEvents.Remove(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
