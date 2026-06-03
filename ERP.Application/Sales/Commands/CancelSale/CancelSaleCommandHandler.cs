using ERP.Application.Common.Interfaces;
using ERP.Domain.Sales.Entities;
using ERP.Domain.Sales.Enums;
using ERP.SharedKernel.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Application.Sales.Commands.CancelSale;

public sealed class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelSaleCommandHandler> _logger;

    public CancelSaleCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CancelSaleCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(CancelSaleCommand command, CancellationToken cancellationToken)
    {
        // STEP 1: Load the sale
        var sale = await _unitOfWork.GetByIdAsync<Sale>(command.SaleId, cancellationToken);
        if (sale is null)
            return Result.Failure($"Sale '{command.SaleId}' was not found.");

        bool isAdmin = command.UserRoles.Contains("Admin");
        bool isManager = command.UserRoles.Contains("Manager");
        bool isEmployee = !isAdmin && !isManager;

        // STEP 2: Role-based status restriction
        // The domain already blocks Shipped / Invoiced / already-Cancelled.
        // We add the business rules on top for Pending and Approved.
        switch (sale.Status)
        {
            case SaleStatus.Approved:
                if (!isAdmin)
                    return Result.Failure("Only Admins can cancel an Approved sale.");
                break;

            case SaleStatus.Pending:
                if (!isAdmin && !isManager)
                    return Result.Failure("Only Admins or Managers can cancel a Pending sale.");
                break;

            case SaleStatus.Draft:
                // All roles may cancel a Draft — ownership check below covers employees
                break;

            default:
                // Shipped, Invoiced, Cancelled — the domain guard inside Cancel() will throw
                break;
        }

        // STEP 3: Employees may only cancel sales they created
        if (isEmployee && sale.CreatedByUserId != command.CancelledByUserId)
            return Result.Failure("You can only cancel your own sales.");

        // STEP 4: Delegate to the domain aggregate
        // (guards: already cancelled, shipped, invoiced, empty reason, empty userId)       // something is fucking with the program in here
        try
        {
            sale.Cancel(command.CancelledByUserId, command.Reason);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        // STEP 5: Persist
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Sale {SaleId} ({SaleNumber}) cancelled by user {UserId}. Reason: {Reason}",
            sale.Id, sale.SaleNumber.Value, command.CancelledByUserId, command.Reason);

        return Result.Success();
    }
}