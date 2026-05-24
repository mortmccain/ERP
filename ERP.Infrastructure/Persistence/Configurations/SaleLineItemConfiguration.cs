using ERP.Domain.Sales.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations;

public sealed class SaleLineItemConfiguration : IEntityTypeConfiguration<SaleLineItem>
{
    public void Configure(EntityTypeBuilder<SaleLineItem> builder)
    {
        // --- Table---
        builder.ToTable(nameof(SaleLineItem) + "s");

        // --- primary Key
        builder.HasKey(sl => sl.Id);
        builder.Property(sl => sl.Id).ValueGeneratedNever();

        // --- Identity & Reference Properties
        builder.Property(li => li.ProductId).IsRequired().ValueGeneratedNever();
        builder.Property(li => li.ProductName).HasMaxLength(500).IsRequired().ValueGeneratedNever();
        builder.Property(li => li.ProductCategory).HasMaxLength(100).IsRequired().ValueGeneratedNever();
        builder.Property(li => li.SKU).HasMaxLength(20).ValueGeneratedNever();
        // we dont set this to not be able to go higher than 10,000 because that's a business rule and it would
        // kinda couple domain to infrastructure more than required
        builder.Property(li => li.Quantity).IsRequired().ValueGeneratedNever();
        builder.OwnsOne(li => li.UnitPrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("UnitPrice").HasColumnType("decimal(18,2)").IsRequired().ValueGeneratedNever();
            money.Property(m => m.Currency).HasColumnName("UnitPriceCurrency").HasMaxLength(3).IsUnicode(false).IsRequired().ValueGeneratedNever();
        });
        builder.Property(li => li.LineNumber).IsRequired().ValueGeneratedNever();

        // --- Line Level Discount --- 
        builder.Property(li => li.DiscountPercentage).HasColumnType("decimal(5,2)").IsRequired().ValueGeneratedNever();
        builder.Property(li => li.DiscountReason).HasMaxLength(500).ValueGeneratedNever();

        // --- FOC ---
        builder.Property(li => li.IsFreeOfCharge).IsRequired().ValueGeneratedNever();
        builder.Property(li => li.FocReason).HasMaxLength(500).ValueGeneratedNever();

        // --- Comaputed Properties ---
        builder.Ignore(li => li.GrossTotal);
        builder.Ignore(li => li.DiscountAmount);
        builder.Ignore(li => li.LineTotal);
    }
}