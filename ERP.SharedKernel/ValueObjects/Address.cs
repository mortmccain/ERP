using ERP.SharedKernel.Primitives;

namespace ERP.SharedKernel.ValueObjects;

public sealed class Address: BaseValueObject
{
    public string Country { get; }
    public string Street { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string ExactAddress { get; }
    public Address(string country,string street, string city, string postalCode, string exactAddress)
    {
        Country = country;
        Street = street;
        City = city;
        PostalCode = postalCode;
        ExactAddress = exactAddress;

    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Country;
        yield return Street;
        yield return City;
        yield return PostalCode;
        yield return ExactAddress;
    }
}
