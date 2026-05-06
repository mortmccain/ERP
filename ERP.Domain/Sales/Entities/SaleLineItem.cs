using ERP.Domain.Sales.ValueObjects;
using ERP.SharedKernel.Primatives;

namespace ERP.Domain.Sales.Entities;

public class SaleLineItem : BaseEntity
{
    public Guid ProductId { get;private set; }
    public string ProductName { get;private set; }

    // don't need it here
    // public string ProductDescription { get;private set;}
    public string ProductCategory { get;private set; }
    public int Quantity { get;private set; }
    public Money UnitPrice { get;private set; }

    // gotta override the * for Money
   // public Money LineTotal => ProductUnitPrice * ProductQuantity;


}
