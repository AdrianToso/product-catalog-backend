using ADR_T.ProductCatalog.Core.Domain.Exceptions;

namespace ADR_T.ProductCatalog.Core.Domain.Entities;

public class Product: EntityBase
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string? ImageUrl { get; private set; }
    public Guid CategoryId { get; private set; } 
    public Category Category { get; private set; } = null!; 


    private Product() { }

    public Product(string name, string description, Guid categoryId, string? imageUrl = null)
    {
        SetName(name);
        SetDescription(description);
        SetCategory(categoryId);
        ImageUrl = imageUrl;
    }

    public void Update(string name, string description, Guid categoryId, string? imageUrl = null)
    {
        SetName(name);
        SetDescription(description);
        SetCategory(categoryId);
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
    private void SetCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new DomainException("La categoría del producto no puede ser un GUID vacío.");
        }
        CategoryId = categoryId;
    }
}

