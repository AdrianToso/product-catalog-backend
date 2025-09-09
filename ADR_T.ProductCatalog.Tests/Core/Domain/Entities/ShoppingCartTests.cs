using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using FluentAssertions;

namespace ADR_T.ProductCatalog.Tests.Core.Domain.Entities;
public class ShoppingCartTests
{
    private readonly Guid _validCustomerId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidCustomerId_ShouldCreateEmptyCart()
    {
        // Act
        var cart = new ShoppingCart(_validCustomerId);

        // Assert
        cart.CustomerId.Should().Be(_validCustomerId);
        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithEmptyCustomerId_ShouldThrowDomainException()
    {
        // Act & Assert
        Action act = () => new ShoppingCart(Guid.Empty);
        act.Should().Throw<DomainException>().WithMessage("Un ShoppingCart debe pertenecer a un Customer.");
    }

    [Fact]
    public void AddItem_WhenItemIsNew_ShouldAddToCart()
    {
        // Arrange
        var cart = new ShoppingCart(_validCustomerId);
        var product = new Product("Test Product", "Desc", 10m, 20, Guid.NewGuid());

        // Act
        cart.AddItem(product, 5);

        // Assert
        cart.Items.Should().HaveCount(1);
        cart.Items.First().ProductId.Should().Be(product.Id);
        cart.Items.First().Quantity.Should().Be(5);
    }

    [Fact]
    public void AddItem_WhenItemExists_ShouldIncreaseQuantity()
    {
        // Arrange
        var cart = new ShoppingCart(_validCustomerId);
        var product = new Product("Test Product", "Desc", 10m, 20, Guid.NewGuid());
        cart.AddItem(product, 5);

        // Act
        cart.AddItem(product, 3);

        // Assert
        cart.Items.Should().HaveCount(1);
        cart.Items.First().Quantity.Should().Be(8);
    }

    [Fact]
    public void AddItem_WithNullProduct_ShouldThrowArgumentNullException()
    {
        // Arrange
        var cart = new ShoppingCart(_validCustomerId);

        // Act & Assert
        Action act = () => cart.AddItem(null!, 5);
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddItem_WithInvalidQuantity_ShouldThrowDomainException(int invalidQuantity)
    {
        // Arrange
        var cart = new ShoppingCart(_validCustomerId);
        var product = new Product("Test Product", "Desc", 10m, 20, Guid.NewGuid());

        // Act & Assert
        Action act = () => cart.AddItem(product, invalidQuantity);
        act.Should().Throw<DomainException>().WithMessage("La cantidad debe ser mayor que cero.");
    }

    [Fact]
    public void AddItem_WhenQuantityExceedsStock_ShouldThrowDomainException()
    {
        // Arrange
        var cart = new ShoppingCart(_validCustomerId);
        var product = new Product("Test Product", "Desc", 10m, 5, Guid.NewGuid());

        // Act & Assert
        Action act = () => cart.AddItem(product, 6);
        act.Should().Throw<DomainException>().WithMessage("No hay suficiente stock para la cantidad solicitada.");
    }

    [Fact]
    public void AddItem_WhenTotalQuantityExceedsStock_ShouldThrowDomainException()
    {
        // Arrange
        var cart = new ShoppingCart(_validCustomerId);
        var product = new Product("Test Product", "Desc", 10m, 5, Guid.NewGuid());
        cart.AddItem(product, 3);

        // Act & Assert
        Action act = () => cart.AddItem(product, 3);
        act.Should().Throw<DomainException>().WithMessage("No hay suficiente stock para la cantidad solicitada.");
    }

    [Fact]
    public void RemoveItem_WhenItemExists_ShouldRemoveFromCart()
    {
        // Arrange
        var cart = new ShoppingCart(_validCustomerId);
        var product = new Product("Test Product", "Desc", 10m, 20, Guid.NewGuid());
        cart.AddItem(product, 5);

        // Act
        cart.RemoveItem(product.Id);

        // Assert
        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public void RemoveItem_WhenItemDoesNotExist_ShouldDoNothing()
    {
        // Arrange
        var cart = new ShoppingCart(_validCustomerId);
        var product = new Product("Test Product", "Desc", 10m, 20, Guid.NewGuid());
        cart.AddItem(product, 5);

        // Act
        Action act = () => cart.RemoveItem(Guid.NewGuid());

        // Assert
        act.Should().NotThrow();
        cart.Items.Should().HaveCount(1);
    }

    [Fact]
    public void UpdateItemQuantity_WhenQuantityIsValid_ShouldUpdateQuantity()
    {
        // Arrange
        var cart = new ShoppingCart(_validCustomerId);
        var product = new Product("Test Product", "Desc", 10m, 20, Guid.NewGuid());
        cart.AddItem(product, 5);

        // Act
        cart.UpdateItemQuantity(product.Id, 15, product.StockQuantity);

        // Assert
        cart.Items.First().Quantity.Should().Be(15);
    }

    [Fact]
    public void UpdateItemQuantity_WhenItemDoesNotExist_ShouldThrowDomainException()
    {
        // Arrange
        var cart = new ShoppingCart(_validCustomerId);
        var product = new Product("Test Product", "Desc", 10m, 20, Guid.NewGuid());

        // Act & Assert
        Action act = () => cart.UpdateItemQuantity(product.Id, 10, product.StockQuantity);
        act.Should().Throw<DomainException>().WithMessage("El producto no se encuentra en el carrito.");
    }

    [Fact]
    public void UpdateItemQuantity_WhenQuantityExceedsStock_ShouldThrowDomainException()
    {
        // Arrange
        var cart = new ShoppingCart(_validCustomerId);
        var product = new Product("Test Product", "Desc", 10m, 20, Guid.NewGuid());
        cart.AddItem(product, 5);

        // Act & Assert
        Action act = () => cart.UpdateItemQuantity(product.Id, 21, product.StockQuantity);
        act.Should().Throw<DomainException>().WithMessage("La cantidad solicitada excede el stock disponible.");
    }

    [Fact]
    public void Clear_ShouldRemoveAllItemsFromCart()
    {
        // Arrange
        var cart = new ShoppingCart(_validCustomerId);
        var product1 = new Product("Product 1", "Desc", 10m, 20, Guid.NewGuid());
        var product2 = new Product("Product 2", "Desc", 15m, 30, Guid.NewGuid());
        cart.AddItem(product1, 2);
        cart.AddItem(product2, 3);

        // Act
        cart.Clear();

        // Assert
        cart.Items.Should().BeEmpty();
    }
}
