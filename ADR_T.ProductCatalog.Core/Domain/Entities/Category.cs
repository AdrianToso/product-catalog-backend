using ADR_T.ProductCatalog.Core.Domain.Exceptions;

namespace ADR_T.ProductCatalog.Core.Domain.Entities;
public class Category: EntityBase
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    // Propiedad de navegación para la relación muchos-a-muchos con Product
    public ICollection<Product> Products { get; private set; } = new List<Product>();
    private Category() { }
    public Category(string name, string? description = null)
    {
        SetName(name);
        Description = description;
    }
    public void Update(string name, string? description = null)
    {
        SetName(name);
        Description = description;
        FechacActualizacion = DateTime.UtcNow;
    }
    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("El nombre de la categoría no puede ser nulo o vacío.");
        }
        if (name.Length > 100)
        {
            throw new DomainException("El nombre de la categoría no puede exceder los 100 caracteres.");
        }
        Name = name;
    }
}
