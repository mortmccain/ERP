using ERP.Application.Common.Interfaces;
using ERP.Domain.Sales.Entities;
using ERP.SharedKernel.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ERP.Application.Sales.Commands.SubmitSale;

public sealed class SubmitSaleCommandHandler : IRequestHandler<SubmitSaleCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubmitSaleCommandHandler> _logger;

    public SubmitSaleCommandHandler(
        ISaleRepository saleRepository,
        IUnitOfWork unitOfWork,
        ILogger<SubmitSaleCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(SubmitSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _unitOfWork.GetByIdAsync<Sale>(
            command.SaleId,
            includes: new[] { nameof(Sale.LineItems) },
            cancellationToken);

        if (sale is null)
            return Result.Failure($"Sale '{command.SaleId}' was not found.");

        bool isAdminOrManager =
            command.UserRoles.Contains("Admin") || command.UserRoles.Contains("Manager");

        // Employees may only submit their own drafts
        if (!isAdminOrManager && sale.CreatedByUserId != command.SubmittedByUserId)
            return Result.Failure("You can only submit your own sales.");

        try
        {
            sale.Submit();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Sale {SaleNumber} (Id: {SaleId}) submitted by user {UserId}.",
            sale.SaleNumber.Value, sale.Id, command.SubmittedByUserId);

        return Result.Success();
    }
}