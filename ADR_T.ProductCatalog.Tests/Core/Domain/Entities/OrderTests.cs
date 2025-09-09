using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Enums;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using FluentAssertions;

namespace ADR_T.ProductCatalog.Tests.Core.Domain.Entities;

public class OrderTests
{
    private readonly Guid _validCustomerId = Guid.NewGuid();
    private readonly Guid _validAddressId = Guid.NewGuid();
    private readonly List<OrderItem> _validOrderItems;

    public OrderTests()
    {
        _validOrderItems = new List<OrderItem>
        {
            new OrderItem(Guid.NewGuid(), Guid.NewGuid(), "Product A", 10.00m, 2), // Total: 20.00
            new OrderItem(Guid.NewGuid(), Guid.NewGuid(), "Product B", 5.50m, 3)   // Total: 16.50
        };
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateOrder()
    {
        // Act
        var order = new Order(_validCustomerId, _validAddressId, _validOrderItems);

        // Assert
        order.CustomerId.Should().Be(_validCustomerId);
        order.ShippingAddressId.Should().Be(_validAddressId);
        order.Status.Should().Be(OrderStatus.Pending);
        order.OrderItems.Should().HaveCount(2);
    }

    [Fact]
    public void Constructor_ShouldCalculateTotalAmountCorrectly()
    {
        // Act
        var order = new Order(_validCustomerId, _validAddressId, _validOrderItems);

        // Assert
        order.TotalAmount.Should().Be(36.50m);
    }

    [Fact]
    public void Constructor_WithEmptyItemsList_ShouldThrowDomainException()
    {
        // Act & Assert
        Action act = () => new Order(_validCustomerId, _validAddressId, new List<OrderItem>());
        act.Should().Throw<DomainException>().WithMessage("Un pedido no puede ser creado sin al menos un ítem.");
    }

    [Fact]
    public void Constructor_WithEmptyCustomerId_ShouldThrowDomainException()
    {
        // Act & Assert
        Action act = () => new Order(Guid.Empty, _validAddressId, _validOrderItems);
        act.Should().Throw<DomainException>().WithMessage("El pedido debe estar asociado a un cliente.");
    }

    [Fact]
    public void Constructor_WithEmptyAddressId_ShouldThrowDomainException()
    {
        // Act & Assert
        Action act = () => new Order(_validCustomerId, Guid.Empty, _validOrderItems);
        act.Should().Throw<DomainException>().WithMessage("El pedido debe tener una dirección de envío.");
    }

    [Fact]
    public void MarkAsPaid_FromPending_ShouldChangeStatusToPaid()
    {
        // Arrange
        var order = new Order(_validCustomerId, _validAddressId, _validOrderItems);

        // Act
        order.MarkAsPaid();

        // Assert
        order.Status.Should().Be(OrderStatus.Paid);
    }

    [Fact]
    public void MarkAsShipped_FromPaid_ShouldChangeStatusToShipped()
    {
        // Arrange
        var order = new Order(_validCustomerId, _validAddressId, _validOrderItems);
        order.MarkAsPaid();

        // Act
        order.MarkAsShipped();

        // Assert
        order.Status.Should().Be(OrderStatus.Shipped);
    }

    [Fact]
    public void MarkAsDelivered_FromShipped_ShouldChangeStatusToDelivered()
    {
        // Arrange
        var order = new Order(_validCustomerId, _validAddressId, _validOrderItems);
        order.MarkAsPaid();
        order.MarkAsShipped();

        // Act
        order.MarkAsDelivered();

        // Assert
        order.Status.Should().Be(OrderStatus.Delivered);
    }

    [Fact]
    public void MarkAsShipped_FromPending_ShouldThrowDomainException()
    {
        // Arrange
        var order = new Order(_validCustomerId, _validAddressId, _validOrderItems);

        // Act & Assert
        Action act = () => order.MarkAsShipped();
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancel_FromShipped_ShouldThrowDomainException()
    {
        // Arrange
        var order = new Order(_validCustomerId, _validAddressId, _validOrderItems);
        order.MarkAsPaid();
        order.MarkAsShipped();

        // Act & Assert
        Action act = () => order.Cancel();
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancel_FromPending_ShouldChangeStatusToCancelled()
    {
        // Arrange
        var order = new Order(_validCustomerId, _validAddressId, _validOrderItems);

        // Act
        order.Cancel();

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
    }
}
