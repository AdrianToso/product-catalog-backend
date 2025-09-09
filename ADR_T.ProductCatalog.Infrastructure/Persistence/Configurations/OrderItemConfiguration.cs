using ADR_T.ProductCatalog.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADR_T.ProductCatalog.Infrastructure.Persistence.Configurations;
public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(oi => oi.UnitPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        // Relación con Product. No se puede eliminar un producto si está en un pedido.
        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
