using ERP.Domain.Sales.Entities;
using ERP.SharedKernel.Common;
using System.Linq.Expressions;

namespace ERP.Application.Sales.Specifications;

public sealed class SaleByCreatorSpecification : Specification<Sale>
{
    private readonly Guid _creatorId;

    public SaleByCreatorSpecification(Guid creatorId)
    {
        _creatorId = creatorId;
    }

    public override Expression<Func<Sale, bool>> ToExpression()
        => sale => sale.CreatedByUserId == _creatorId;
}