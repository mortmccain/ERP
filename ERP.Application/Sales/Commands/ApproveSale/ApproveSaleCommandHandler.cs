using ERP.Application.Common.Interfaces;
using ERP.Domain.Sales.Entities;
using ERP.SharedKernel.Common;
using Microsoft.Extensions.Logging;

namespace ERP.Application.Sales.Commands.ApproveSale;

public sealed class ApproveSaleCommandHandler : IRequestHandler<ApproveSaleCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveSaleCommandHandler> _logger;

    public ApproveSaleCommandHandler
        (
        IUnitOfWork unitOfWork,
        ILogger<ApproveSaleCommandHandler> logger
        )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(ApproveSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _unitOfWork.GetByIdAsync<Sale>(command.SaleId, cancellationToken);

        if (sale is null)
            return Result.Failure($"Sale '{command.SaleId}' was not found.");

        bool canApprove =
            command.UserRoles.Contains("Admin") || command.UserRoles.Contains("Manager");

        if (!canApprove)
            return Result.Failure("Only Admins and Managers can approve sales.");

        try
        {
            sale.Approve(command.ApprovedByUserId);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Sale {SaleNumber} (Id: {SaleId}) approved by user {UserId}.",
            sale.SaleNumber.Value, sale.Id, command.ApprovedByUserId);

        return Result.Success();
    }
}