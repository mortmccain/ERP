using ERP.Application.Common.Interfaces;
using ERP.Domain.Sales.Entities;
using ERP.SharedKernel.Common;
using Microsoft.Extensions.Logging;

namespace ERP.Application.Sales.Commands.MarkAsShipped;

public static class MarkAsShippedCommandHandler
{
    public static async Task<Result> Handle
        (
        MarkAsShippedCommand command,
        IUnitOfWork unitOfWork,
        ILogger<MarkAsShippedCommand> logger,
        CancellationToken cancellationToken
        )
    {
        var sale = await unitOfWork.GetByIdAsync<Sale>(command.SaleId, cancellationToken: cancellationToken);
        if (sale is null)
            return Result.Failure($"Sale '{command.SaleId}' was not found.");

        // Only Admin and Manager can mark a sale as shipped
        if (!command.UserRoles.Contains("Admin") && !command.UserRoles.Contains("Manager"))
            return Result.Failure("Only Admins and Managers can mark a sale as shipped.");

        try
        {
            sale.MarkAsShipped();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Sale {SaleNumber} (Id: {SaleId}) marked as shipped by user {UserId}.",
            sale.SaleNumber.Value, sale.Id, command.MarkedByUserId);

        return Result.Success();
    }
}