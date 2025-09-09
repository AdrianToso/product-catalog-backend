using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADR_T.ProductCatalog.Infrastructure.Persistence.Configurations;
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion(
                s => s.ToString(),
                s => (OrderStatus)Enum.Parse(typeof(OrderStatus), s)
            )
            .HasMaxLength(50);

        // Relación con Customer (requerida).
        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .IsRequired();

        // Relación con Address (requerida y restringida al borrado).
        builder.HasOne(o => o.ShippingAddress)
            .WithMany()
            .HasForeignKey(o => o.ShippingAddressId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // Relación 1-a-N con OrderItems (borrado en cascada).
        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indicar a EF Core que use el campo privado "_orderItems".
        builder.Navigation(o => o.OrderItems)
            .HasField("_orderItems")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
