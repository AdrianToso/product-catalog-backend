using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using FluentAssertions;

namespace ADR_T.ProductCatalog.Tests.Core.Domain.Entities;

public class CartItemTests
{
    private readonly Guid _validCartId = Guid.NewGuid();
    private readonly Guid _validProductId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateCartItem()
    {
        // Arrange
        var quantity = 5;

        // Act
        var cartItem = new CartItem(_validCartId, _validProductId, quantity);

        // Assert
        cartItem.ShoppingCartId.Should().Be(_validCartId);
        cartItem.ProductId.Should().Be(_validProductId);
        cartItem.Quantity.Should().Be(quantity);
    }

    [Fact]
    public void Constructor_WithEmptyShoppingCartId_ShouldThrowDomainException()
    {
        // Act & Assert
        Action act = () => new CartItem(Guid.Empty, _validProductId, 5);
        act.Should().Throw<DomainException>()
           .WithMessage("El CartItem debe estar asociado a un ShoppingCart.");
    }

    [Fact]
    public void Constructor_WithEmptyProductId_ShouldThrowDomainException()
    {
        // Act & Assert
        Action act = () => new CartItem(_validCartId, Guid.Empty, 5);
        act.Should().Throw<DomainException>()
           .WithMessage("El CartItem debe estar asociado a un Product.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidQuantity_ShouldThrowDomainException(int invalidQuantity)
    {
        // Act & Assert
        Action act = () => new CartItem(_validCartId, _validProductId, invalidQuantity);
        act.Should().Throw<DomainException>()
           .WithMessage("La cantidad (Quantity) de un CartItem debe ser mayor que cero.");
    }

    [Fact]
    public void UpdateQuantity_WithValidQuantity_ShouldUpdateTheQuantity()
    {
        // Arrange
        var cartItem = new CartItem(_validCartId, _validProductId, 5);
        var initialUpdateDate = cartItem.FechacActualizacion;

        // Act
        cartItem.UpdateQuantity(10);

        // Assert
        cartItem.Quantity.Should().Be(10);
        cartItem.FechacActualizacion.Should().NotBe(initialUpdateDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UpdateQuantity_WithInvalidQuantity_ShouldThrowDomainException(int invalidQuantity)
    {
        // Arrange
        var cartItem = new CartItem(_validCartId, _validProductId, 5);

        // Act & Assert
        Action act = () => cartItem.UpdateQuantity(invalidQuantity);
        act.Should().Throw<DomainException>()
           .WithMessage("La cantidad (Quantity) de un CartItem debe ser mayor que cero.");
    }
}
