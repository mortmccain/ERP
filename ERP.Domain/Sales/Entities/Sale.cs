
using ERP.Domain.Sales.ValueObjects;
using ERP.SharedKernel.Primatives;

namespace ERP.Domain.Sales.Entities;

public class Sale
{
    public SaleNumber SaleNumber { get; private set; }
    public string CustomerName { get; private set; }
    public Address ShippingAddress { get; private set; }
    public Money Total {  get; private set; }
    /// <summary>
    /// we have total and subtotal so we can add tax and discounts easily to the total : total => subTotal + tax - discount
    /// </summary>
    public Money SubTotal {  get; private set; }



}
