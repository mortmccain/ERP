using ERP.Domain.Sales.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the Sale Aggregate Root.
/// Maps the domain entity to database tables and columns.
/// </summary>
public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        // --- Table ---
        builder.ToTable(nameof(Sale) + "s");

        // --- Primary Key ---
        builder.HasKey(s => s.Id);      // primary keys are required by default so no need for IsRequired()
        builder.Property(s => s.Id).ValueGeneratedNever(); // Domain generates GUIDs, not the database

        // --- Identity & Reference Properties ---
        builder.OwnsOne(s => s.SaleNumber, sn =>
        {
            sn.Property(n => n.Value)
                .HasColumnName("SaleNumber")
                .HasMaxLength(14)
                .IsRequired();
            sn.HasIndex(n => n.Value).IsUnique();
            sn.Ignore(c => c.Prefix);
            sn.Ignore(c => c.Year);
            sn.Ignore(c => c.Sequence);
        });

        builder.Property(s => s.CustomerId).IsRequired().ValueGeneratedNever();
        builder.Property(s => s.CustomerName).HasMaxLength(200).IsRequired().ValueGeneratedNever();
        builder.Property(s => s.CreatedByUserId).IsRequired().ValueGeneratedNever();
        builder.Property(s => s.CreatedByName).HasMaxLength(200).IsRequired().ValueGeneratedNever();

        // --- Shipping ---
        builder.OwnsOne(s => s.ShippingAddress, address =>
        {
            address.Property(a => a.Country).HasColumnName("ShippingCountry").HasMaxLength(100).IsRequired();
            address.Property(a => a.Street).HasColumnName("ShippingStreet").HasMaxLength(100).IsRequired().ValueGeneratedNever();
            address.Property(a => a.City).HasColumnName("ShippingCity").HasMaxLength(200).IsRequired().ValueGeneratedNever();
            address.Property(a => a.PostalCode).HasColumnName("ShippingPostalCode").HasMaxLength(10).IsRequired().ValueGeneratedNever();
            address.Property(a => a.ExactAddress).HasColumnName("ExactShippingAddress").HasMaxLength(500).IsRequired().ValueGeneratedNever();
        });

        // --- Status ---
        builder.Property(s => s.Status)
            .HasConversion<int>() // Store enum as integer in the database
            .IsRequired().ValueGeneratedNever();

        // --- Financials (Owned Value Objects stored in the same table) ---
        builder.OwnsOne(s => s.SubTotal, money =>
        {
            money.Property(m => m.Amount).HasColumnName("SubTotalAmount").HasColumnType("decimal(18,2)").IsRequired().ValueGeneratedNever();
            money.Property(m => m.Currency).HasColumnName("SubTotalCurrency").HasMaxLength(3).IsUnicode(false).IsRequired().ValueGeneratedNever();
        });

        builder.OwnsOne(s => s.SubTotalAfterLineDiscounts, money =>
        {
            money.Property(m => m.Amount).HasColumnName("SubTotalAfterDiscountAmount").HasColumnType("decimal(18,2)").IsRequired().ValueGeneratedNever();
            money.Property(m => m.Currency).HasColumnName("SubTotalAfterDiscountCurrency").HasMaxLength(3).IsUnicode(false).IsRequired().ValueGeneratedNever();
        });

        builder.OwnsOne(s => s.Discount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("DiscountAmount").HasColumnType("decimal(18,2)").IsRequired().ValueGeneratedNever();
            money.Property(m => m.Currency).HasColumnName("DiscountCurrency").HasMaxLength(3).IsUnicode(false).IsRequired().ValueGeneratedNever();
        });

        builder.OwnsOne(s => s.TaxableAmount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("TaxableAmountAmount").HasColumnType("decimal(18,2)").IsRequired().ValueGeneratedNever();
            money.Property(m => m.Currency).HasColumnName("TaxableAmountCurrency").HasMaxLength(3).IsUnicode(false).IsRequired().ValueGeneratedNever();
        });

        builder.OwnsOne(s => s.Tax, money =>
        {
            money.Property(m => m.Amount).HasColumnName("TaxAmount").HasColumnType("decimal(18,2)").IsRequired().ValueGeneratedNever();
            money.Property(m => m.Currency).HasColumnName("TaxCurrency").HasMaxLength(3).IsUnicode(false).IsRequired().ValueGeneratedNever();
        });

        builder.OwnsOne(s => s.Total, money =>
        {
            money.Property(m => m.Amount).HasColumnName("TotalAmount").HasColumnType("decimal(18,2)").IsRequired().ValueGeneratedNever();
            money.Property(m => m.Currency).HasColumnName("TotalCurrency").HasMaxLength(3).IsUnicode(false).IsRequired().ValueGeneratedNever();
        });

        // --- Discount Metadata ---
        builder.Property(s => s.DiscountPercentage).HasColumnType("decimal(5,2)").ValueGeneratedNever();
        builder.Property(s => s.DiscountReason).HasMaxLength(500).ValueGeneratedNever();

        // --- Tax Metadata ---
        builder.Property(s => s.TaxRate).HasColumnType("decimal(5,2)").ValueGeneratedNever();

        // --- Timestamps ---
        builder.Property(s => s.CreatedAtUtc).IsRequired().ValueGeneratedNever();
        builder.Property(s => s.ApprovedAtUtc).ValueGeneratedNever();
        builder.Property(s => s.ApprovedByUserId).ValueGeneratedNever();
        builder.Property(s => s.ShippedAtUtc).ValueGeneratedNever();
        builder.Property(s => s.CancelledAtUtc).ValueGeneratedNever();
        builder.Property(s => s.CancellationReason).HasMaxLength(1000).ValueGeneratedNever();

        // --- Line Items (One-to-Many relationship) ---
        builder.HasMany(s => s.LineItems)                    // has many sale line items
            .WithOne()                                      // each sale line item belongs to one sale
            .HasForeignKey("SaleId")                       // link them via the SaleId column
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Sale.LineItems))!        // this line says go find LineItems
            .SetPropertyAccessMode(PropertyAccessMode.Field);          // this line says don't use LineItems, use the backing field

        // --- Ignore Domain Events (not persisted) ---     
        // weren't we supposed to persist domain events so if something happened to the server we can still do stuff like
        // send emails and do whatever that's required after some command / query is done?
        builder.Ignore(s => s.DomainEvents);
    }
}