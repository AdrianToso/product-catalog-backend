using ADR_T.ProductCatalog.Core.Domain.Exceptions;

namespace ADR_T.ProductCatalog.Core.Domain.Entities;
public class OrderItem : EntityBase
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    // Navigation property
    public Order Order { get; private set; } = null!;

    private OrderItem() { }

    public OrderItem(Guid orderId, Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (orderId == Guid.Empty)
            throw new DomainException("El OrderItem debe estar asociado a un Order.");

        if (productId == Guid.Empty)
            throw new DomainException("El OrderItem debe estar asociado a un Product.");

        if (string.IsNullOrWhiteSpace(productName))
            throw new DomainException("El nombre del producto (ProductName) es requerido como snapshot.");

        if (unitPrice < 0)
            throw new DomainException("El precio unitario (UnitPrice) no puede ser negativo.");

        if (quantity <= 0)
            throw new DomainException("La cantidad (Quantity) debe ser mayor que cero.");

        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}
