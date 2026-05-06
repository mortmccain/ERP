using ERP.SharedKernel.Primatives;

namespace ERP.Domain.Sales.ValueObjects;

public class SaleNumber :BaseValueObject
{
    public string Prefix { get; } // "SALE"
    public int Year { get; }      // 2024
    public int Sequence { get; }  // 42
    public string Value { get; }

    private SaleNumber(string prefix, int year, int sequence)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new ArgumentException("Prefix is required");
        if (year < 2000 || year > 2100)
            throw new ArgumentException("Invalid year");
        if (sequence < 1 || sequence > 9999)
            throw new ArgumentException("Sequence must be between 1 and 9999");

        Prefix = prefix.ToUpper();
        Year = year;
        Sequence = sequence;
        Value = $"{Prefix}-{Year}-{Sequence:D4}";
    }

    public static SaleNumber Next(SaleNumber previous, string prefix)
    {
        if (previous.Year != DateTime.Now.Year)
            return new SaleNumber(prefix, DateTime.Now.Year, 1);
        return new SaleNumber(prefix, previous.Year, previous.Sequence + 1);
    }

    public override string ToString()
    {
        return $"{Prefix}-{Year}-{Sequence:D4}";
    }

    public bool IsInternationalSale() => Prefix == "INT";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Prefix;
        yield return Year;
        yield return Sequence; 
    }
}
