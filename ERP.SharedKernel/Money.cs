using ERP.SharedKernel.Primatives;

namespace ERP.Domain.Sales.ValueObjects;

public class Money :BaseValueObject
{
    //the whole null issue. rethink this later
    public string Currency { get; }
    public decimal Amount { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Currency;
        yield return Amount;
    }
}