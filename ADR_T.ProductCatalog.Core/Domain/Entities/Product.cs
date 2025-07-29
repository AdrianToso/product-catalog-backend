using ADR_T.ProductCatalog.Core.Domain.Exceptions;

namespace ADR_T.ProductCatalog.Core.Domain.Entities;

public class Product: EntityBase
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string? ImageUrl { get; private set; }

    // Propiedad de navegación para la relación muchos-a-muchos con Category
    public ICollection<Category> Categories { get; private set; } = new List<Category>();

    private Product() { }

    public Product(string name, string description, string? imageUrl = null)
    {
        SetName(name);
        SetDescription(description);
        ImageUrl = imageUrl;
    }

    public void Update(string name, string description, string? imageUrl = null)
    {
        SetName(name);
        SetDescription(description);
        ImageUrl = imageUrl;
        FechacActualizacion = DateTime.UtcNow;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("El nombre del producto no puede ser nulo o vacío.");
        }
        if (name.Length > 200)
        {
            throw new DomainException("El nombre del producto no puede exceder los 200 caracteres.");
        }
        Name = name;
    }

    private void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("La descripción del producto no puede ser nula o vacía.");
        }
        Description = description;
    }
}
