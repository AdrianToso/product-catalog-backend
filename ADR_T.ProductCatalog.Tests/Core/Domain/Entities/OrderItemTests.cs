using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using FluentAssertions;

namespace ADR_T.ProductCatalog.Tests.Core.Domain.Entities;
public class OrderItemTests
{
    private readonly Guid _validOrderId = Guid.NewGuid();
    private readonly Guid _validProductId = Guid.NewGuid();
    private const string ValidProductName = "Test Product";
    private const decimal ValidPrice = 10.00m;
    private const int ValidQuantity = 2;

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateOrderItem()
    {
        // Act
        var orderItem = new OrderItem(_validOrderId, _validProductId, ValidProductName, ValidPrice, ValidQuantity);

        // Assert
        orderItem.OrderId.Should().Be(_validOrderId);
        orderItem.ProductId.Should().Be(_validProductId);
        orderItem.ProductName.Should().Be(ValidProductName);
        orderItem.UnitPrice.Should().Be(ValidPrice);
        orderItem.Quantity.Should().Be(ValidQuantity);
    }

    [Fact]
    public void Constructor_WithEmptyOrderId_ShouldThrowDomainException()
    {
        // Act & Assert
        Action act = () => new OrderItem(Guid.Empty, _validProductId, ValidProductName, ValidPrice, ValidQuantity);
        act.Should().Throw<DomainException>().WithMessage("El OrderItem debe estar asociado a un Order.");
    }

    [Fact]
    public void Constructor_WithEmptyProductId_ShouldThrowDomainException()
    {
        // Act & Assert
        Action act = () => new OrderItem(_validOrderId, Guid.Empty, ValidProductName, ValidPrice, ValidQuantity);
        act.Should().Throw<DomainException>().WithMessage("El OrderItem debe estar asociado a un Product.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidProductName_ShouldThrowDomainException(string? invalidName)
    {
        // Act & Assert
        Action act = () => new OrderItem(_validOrderId, _validProductId, invalidName, ValidPrice, ValidQuantity);
        act.Should().Throw<DomainException>().WithMessage("El nombre del producto (ProductName) es requerido como snapshot.");
    }

    [Fact]
    public void Constructor_WithNegativePrice_ShouldThrowDomainException()
    {
        // Act & Assert
        Action act = () => new OrderItem(_validOrderId, _validProductId, ValidProductName, -1.00m, ValidQuantity);
        act.Should().Throw<DomainException>().WithMessage("El precio unitario (UnitPrice) no puede ser negativo.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidQuantity_ShouldThrowDomainException(int invalidQuantity)
    {
        // Act & Assert
        Action act = () => new OrderItem(_validOrderId, _validProductId, ValidProductName, ValidPrice, invalidQuantity);
        act.Should().Throw<DomainException>().WithMessage("La cantidad (Quantity) debe ser mayor que cero.");
    }
}
