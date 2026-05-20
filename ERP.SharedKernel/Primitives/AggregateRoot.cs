using ERP.SharedKernel.Common;

namespace ERP.SharedKernel.Primitives;

/// <summary>
/// Base class for Aggregate Roots. Manages a collection of Domain Events
/// that are dispatched when the Unit of Work saves changes.
/// </summary>
public abstract class AggregateRoot : BaseEntity
{
    private readonly List<BaseDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot(Guid id) : base(id) { }
    protected AggregateRoot() : base() { }

    protected void AddDomainEvent(BaseDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}