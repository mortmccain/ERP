using ERP.Domain.Customers.Entities;
using ERP.SharedKernel.Common;
using System.Linq.Expressions;

namespace ERP.Application.Customers.Specifications;

public sealed class ActiveCustomerSpecification : Specification<Customer>
{
    public override Expression<Func<Customer, bool>> ToExpression()
        => customer => customer.IsActive;
}