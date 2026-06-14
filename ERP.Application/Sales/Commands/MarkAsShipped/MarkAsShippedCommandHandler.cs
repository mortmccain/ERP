using ERP.Application.Common.Interfaces;
using ERP.Domain.Sales.Entities;
using ERP.SharedKernel.Common;
using Microsoft.Extensions.Logging;

namespace ERP.Application.Sales.Commands.MarkAsShipped;

public sealed class MarkAsShippedCommandHandler : IRequestHandler<MarkAsShippedCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MarkAsShippedCommandHandler> _logger;

    public MarkAsShippedCommandHandler
        (
        IUnitOfWork unitOfWork,
        ILogger<MarkAsShippedCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(MarkAsShippedCommand command, CancellationToken cancellationToken)
    {
        var sale = await _unitOfWork.GetByIdAsync<Sale>(command.SaleId, cancellationToken: cancellationToken);
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

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Sale {SaleNumber} (Id: {SaleId}) marked as shipped by user {UserId}.",
            sale.SaleNumber.Value, sale.Id, command.MarkedByUserId);

        return Result.Success();
    }
}