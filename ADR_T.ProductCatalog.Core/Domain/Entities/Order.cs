using ADR_T.ProductCatalog.Core.Domain.Enums;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;

namespace ADR_T.ProductCatalog.Core.Domain.Entities;
public class Order : EntityBase
{
    public Guid CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public Guid ShippingAddressId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }

    // Navigation properties
    public Customer Customer { get; private set; } = null!;
    public Address ShippingAddress { get; private set; } = null!;

    private readonly List<OrderItem> _orderItems = new List<OrderItem>();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    private Order() { }

    public Order(Guid customerId, Guid shippingAddressId, List<OrderItem> orderItems)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("El pedido debe estar asociado a un cliente.");

        if (shippingAddressId == Guid.Empty)
            throw new DomainException("El pedido debe tener una dirección de envío.");

        if (orderItems == null || !orderItems.Any())
            throw new DomainException("Un pedido no puede ser creado sin al menos un ítem.");

        CustomerId = customerId;
        ShippingAddressId = shippingAddressId;
        _orderItems = orderItems;
        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.Pending;

        CalculateTotalAmount();
    }

    private void CalculateTotalAmount()
    {
        TotalAmount = _orderItems.Sum(item => item.UnitPrice * item.Quantity);
    }

    public void MarkAsPaid()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new DomainException($"No se puede marcar como 'Pagado' un pedido en estado '{Status}'.");
        }
        Status = OrderStatus.Paid;
        FechacActualizacion = DateTime.UtcNow;
    }

    public void MarkAsShipped()
    {
        if (Status != OrderStatus.Paid)
        {
            throw new DomainException($"No se puede marcar como 'Enviado' un pedido que no ha sido pagado (estado actual: '{Status}').");
        }
        Status = OrderStatus.Shipped;
        FechacActualizacion = DateTime.UtcNow;
    }

    public void MarkAsDelivered()
    {
        if (Status != OrderStatus.Shipped)
        {
            throw new DomainException($"No se puede marcar como 'Entregado' un pedido que no ha sido enviado (estado actual: '{Status}').");
        }
        Status = OrderStatus.Delivered;
        FechacActualizacion = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
        {
            throw new DomainException($"No se puede cancelar un pedido que ya fue enviado o entregado (estado actual: '{Status}').");
        }
        Status = OrderStatus.Cancelled;
        FechacActualizacion = DateTime.UtcNow;
    }
}
