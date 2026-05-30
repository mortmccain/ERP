using ERP.Application.Common.Interfaces;
using ERP.Domain.Customers.Entities;
using ERP.Domain.Sales.Entities;
using ERP.SharedKernel.Common;
using ERP.SharedKernel.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Application.Sales.Commands.CreateSale;

/// <summary>
/// Handles the creation of a new Sale.
/// Orchestrates loading the Customer, creating the Sale Aggregate,
/// adding line items, and persisting everything.
/// </summary>
public sealed class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Result<Guid>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ISaleNumberGenerator _saleNumberGenerator;
    private readonly ISaleRepository _saleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateSaleCommandHandler> _logger;

    public CreateSaleCommandHandler
        (
        ICustomerRepository customerRepository,
        ISaleNumberGenerator saleNumberGenerator,
        ISaleRepository saleRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateSaleCommandHandler> logger
        )
    {
        _customerRepository = customerRepository;
        _saleNumberGenerator = saleNumberGenerator;
        _saleRepository = saleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle
        (
        CreateSaleCommand command,
        CancellationToken cancellationToken
        )
    {
        // ------------------------------------------------------------------
        // STEP 1: Load the Customer
        // ------------------------------------------------------------------
        Customer? customer = await _customerRepository.GetByIdAsync
            (
            command.CustomerId, cancellationToken
            );

        if (customer is null)
        {
            return Result<Guid>.Failure($"Customer with ID '{command.CustomerId}' was not found.");
        }

        if (!customer.IsActive)
        {
            return Result<Guid>.Failure($"Customer '{customer.Name}' is inactive. Cannot create a sale.");
        }

        // ------------------------------------------------------------------
        // STEP 2: Generate a SaleNumber
        // ------------------------------------------------------------------
        // In production, this would use a sequence or a domain service.
        // For now, we generate a simple sequential-ish number.
        var saleNumber = await _saleNumberGenerator.NextAsync("SALE", cancellationToken);
        // ------------------------------------------------------------------
        // STEP 3: Create the Sale Aggregate
        // ------------------------------------------------------------------
        var sale = Sale.Create
            (
            // shouldn't we take some of these from the database for security reasons? _currentUserService.UserId 
            command.CustomerId,
            customer.Name,      
            customer.ShippingAddress,
            saleNumber,
            command.CreatedByUserId,
            command.CreatedByName
            );

        // ------------------------------------------------------------------
        // STEP 4: Add line items to the Sale
        // ------------------------------------------------------------------
        foreach (var item in command.Items)
        {
            var unitPrice = new Money(item.UnitPriceAmount, item.Currency);

            sale.AddLineItem
                (
                item.ProductId,
                item.ProductName,
                item.SKU,
                item.Quantity,
                unitPrice,
                productCategory: "General"      // Could come from Product aggregate in production
                ); 
        }

        // ------------------------------------------------------------------
        // STEP 5: Persist
        // ------------------------------------------------------------------
        _saleRepository.Add(sale);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // ------------------------------------------------------------------
        // STEP 6: Log and return
        // ------------------------------------------------------------------
        _logger.LogInformation
            (
            "Created Sale {SaleNumber} for Customer {CustomerName}. SaleId: {SaleId}",
            sale.SaleNumber,
            customer.Name,
            sale.Id
            );

        return Result<Guid>.Success(sale.Id);
    }
}