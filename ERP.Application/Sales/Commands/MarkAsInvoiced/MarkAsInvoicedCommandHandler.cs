using ERP.Application.Common.Interfaces;
using ERP.Domain.Sales.Entities;
using ERP.SharedKernel.Common;
using Microsoft.Extensions.Logging;

namespace ERP.Application.Sales.Commands.MarkAsInvoiced;

public sealed class MarkAsInvoicedCommandHandler : IRequestHandler<MarkAsInvoicedCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MarkAsInvoicedCommandHandler> _logger;

    public MarkAsInvoicedCommandHandler
        (
        IUnitOfWork unitOfWork,
        ILogger<MarkAsInvoicedCommandHandler> logger
        )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(MarkAsInvoicedCommand command, CancellationToken cancellationToken)
    {
        var sale = await _unitOfWork.GetByIdAsync<Sale>(command.SaleId, cancellationToken: cancellationToken);
        if (sale is null)
            return Result.Failure($"Sale '{command.SaleId}' was not found.");

        if (!command.UserRoles.Contains("Admin") && !command.UserRoles.Contains("Manager"))
            return Result.Failure("Only Admins and Managers can mark a sale as invoiced.");

        try
        {
            sale.MarkAsInvoiced();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Sale {SaleNumber} (Id: {SaleId}) marked as invoiced by user {UserId}.",
            sale.SaleNumber.Value, sale.Id, command.MarkedByUserId);

        return Result.Success();
    }
}