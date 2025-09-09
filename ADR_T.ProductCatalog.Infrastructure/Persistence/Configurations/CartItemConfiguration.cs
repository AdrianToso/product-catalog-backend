using ADR_T.ProductCatalog.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADR_T.ProductCatalog.Infrastructure.Persistence.Configurations;
public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Quantity)
            .IsRequired();

        // Relación N-a-1: Muchos CartItems referencian a un único Product.
        builder.HasOne(ci => ci.Product)
            .WithMany() // Product no necesita una lista de CartItems.
            .HasForeignKey(ci => ci.ProductId)
            .IsRequired();
    }
}
