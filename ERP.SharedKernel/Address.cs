using ERP.SharedKernel.Primatives;

namespace ERP.Domain.Sales.ValueObjects;

public class Address: BaseValueObject
{
    public string Country { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string ExactAddress { get; }
    public Address(string country, string city, string postalCode, string exactAddress)
    {
        Country = country;
        City = city;
        PostalCode = postalCode;
        ExactAddress = exactAddress;

    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Country;
        yield return City;
        yield return PostalCode;
        yield return ExactAddress;
    }
}
