using ERP.Application.Common.Interfaces;
using ERP.Domain.Sales.Events;
using Microsoft.Extensions.Logging;

namespace ERP.Application.Sales.EventHandlers;

/// <summary>
/// When a Sale is created, updates the Customer's LastOrderDateUtc.
/// This now correctly runs against the shared DbContext from UnitOfWork 
/// (same transaction as the Sale creation).
/// </summary>
public sealed class UpdateCustomerLastOrderDateHandler
    : INotificationHandler<SaleCreatedDomainEvent>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateCustomerLastOrderDateHandler> _logger;

    public UpdateCustomerLastOrderDateHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateCustomerLastOrderDateHandler> logger)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(
        SaleCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(
            domainEvent.CustomerId, cancellationToken);

        if (customer is null)
        {
            _logger.LogWarning(
                "Customer {CustomerId} not found when updating LastOrderDate for Sale {SaleId}",
                domainEvent.CustomerId, domainEvent.SaleId);
            return;
        }

        customer.RecordOrder(domainEvent.CreatedAtUtc);

        _logger.LogInformation(
            "Updated LastOrderDate for Customer {CustomerId} to {OrderDate}",
            customer.Id, domainEvent.CreatedAtUtc);
    }
}