using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using System.Diagnostics;

namespace ADR_T.ProductCatalog.Core.Domain.Entities;

public class Product : EntityBase
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;


    private Product() { }

    public Product(string name, string description, decimal price, int stockQuantity, Guid categoryId, string? imageUrl = null)
    {
        SetName(name);
        SetDescription(description);
        SetPrice(price);
        SetStockQuantity(stockQuantity);
        SetCategory(categoryId);
        ImageUrl = imageUrl;
    }

    public void Update(string name, string description, decimal price, int stockQuantity, Guid categoryId, string? imageUrl = null)
    {
        SetName(name);
        SetDescription(description);
        SetPrice(price);
        SetStockQuantity(stockQuantity);
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
    public void SetImageUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new DomainException("La URL de la imagen no puede ser vacía.");

        ImageUrl = url;
        FechacActualizacion = DateTime.UtcNow;
    }
    private void SetPrice(decimal price)
    {
        if (price < 0)
        {
            throw new DomainException("El precio no puede ser negativo.");
        }
        Price = price;
    }
    private void SetStockQuantity(int stockQuantity)
    {
        if (stockQuantity < 0)
        {
            throw new DomainException("La cantidad en stock no puede ser negativa.");
        }
        StockQuantity = stockQuantity;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException("La cantidad para añadir al stock debe ser positiva.");
        }
        SetStockQuantity(StockQuantity + quantity);
        FechacActualizacion = DateTime.UtcNow;
    }
    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException("La cantidad para remover del stock debe ser positiva.");
        }
        if (StockQuantity < quantity)
        {
            throw new DomainException("No hay suficiente stock para remover la cantidad solicitada.");
        }
        SetStockQuantity(StockQuantity - quantity);
        FechacActualizacion = DateTime.UtcNow;
    }
}

