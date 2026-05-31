using ERP.SharedKernel.Primitives;

namespace ERP.Domain.Customers.ValueObjects;

public sealed class CustomerCode : BaseValueObject
{
    public string Prefix { get; }
    public int Year { get; }
    public int Sequence { get; }
    public string Value { get; }

    private CustomerCode(string prefix, int year, int sequence)
    {
        if (string.IsNullOrWhiteSpace(prefix)) throw new ArgumentException("Prefix is required");

        if (year < 200 || year > 2100) throw new ArgumentException("Invalid Year");

        if (sequence < 1 || sequence > 9999)
            throw new ArgumentException("How did we get so many customers this year alone, and are still using this program?");

        Prefix = prefix.ToUpperInvariant();
        Year = year;
        Sequence = sequence;
        // D4 make it shot small numbers like : 0042
        Value = $"{Prefix}-{Year}-{Sequence:D4}";
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private CustomerCode() { }
#pragma warning restore CS8618
    public static CustomerCode First(string prefix)
    {
        return new CustomerCode(prefix, DateTime.Now.Year, 1);
    }

    public static CustomerCode Next(CustomerCode previous, string prefix)
    {
        if (previous.Year != DateTime.Now.Year) return First(prefix);
        return new CustomerCode(prefix, previous.Year, previous.Sequence + 1);
    }

    public static CustomerCode FromParts(string prefix, int year, int sequence)
    {
        return new CustomerCode(prefix, year, sequence);
    }

    public override string ToString() => Value;

    public bool IsPerson() => Prefix == "HAGH";

    public bool IsCompany() => Prefix == "HOGH";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Prefix;
        yield return Year;
        yield return Sequence;
    }
}