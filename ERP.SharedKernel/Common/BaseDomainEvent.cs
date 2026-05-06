using MediatR;
//install MediatorR.Contracts
namespace ERP.SharedKernel.Common
{
    /// <summary>
    /// Base class for all Domain Events. Implements INotification for MediatR dispatch.
    /// </summary>
    public abstract class BaseDomainEvent : INotification
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
