using ADR_T.ProductCatalog.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ADR_T.ProductCatalog.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .IsRequired();

        // Relación implícita con AspNetUsers a través de UserId.
        // EF Core la manejará por convención, pero no se define una clave foránea directa a la tabla de Identity aquí.

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);

        // Relación 1-a-N: Un Customer tiene muchos Orders.
        builder.HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict); // Evita eliminar un cliente si tiene pedidos.

        // Relación 1-a-N: Un Customer tiene muchas Addresses.
        builder.HasMany<Address>() // Se define la relación aunque no haya una propiedad de navegación en Customer.
            .WithOne(a => a.Customer)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade); // Al eliminar un cliente, se eliminan sus direcciones.
    }
}
