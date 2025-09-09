using ADR_T.ProductCatalog.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADR_T.ProductCatalog.Infrastructure.Persistence.Configurations;
public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.ToTable("ShoppingCarts");

        builder.HasKey(sc => sc.Id);

        // Relación 1-a-1: Un ShoppingCart pertenece a un único Customer.
        builder.HasOne(sc => sc.Customer)
            .WithOne() // No hay propiedad de navegación de vuelta desde Customer.
            .HasForeignKey<ShoppingCart>(sc => sc.CustomerId)
            .IsRequired();

        // Relación 1-a-N: Un ShoppingCart tiene muchos CartItems.
        builder.HasMany(sc => sc.Items)
            .WithOne(ci => ci.ShoppingCart)
            .HasForeignKey(ci => ci.ShoppingCartId)
            .OnDelete(DeleteBehavior.Cascade); // Si se borra el carrito, se borran sus ítems.

        // Indicar a EF Core que use el campo privado "_items" para la colección.
        builder.Navigation(sc => sc.Items)
            .HasField("_items")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
