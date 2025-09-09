using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using FluentAssertions;
using Xunit;
using System;

namespace ADR_T.ProductCatalog.Tests.Core.Domain.Entities
{
    public class ProductTests
    {
        private readonly Guid _validCategoryId = Guid.NewGuid();
        private const string ValidName = "Test Product";
        private const string ValidDescription = "Test Description";
        private const decimal ValidPrice = 99.99m;
        private const int ValidStock = 100;
        private const string ValidImageUrl = "https://example.com/image.jpg";

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateProduct()
        {
            // Act
            var product = new Product(ValidName, ValidDescription, ValidPrice, ValidStock, _validCategoryId, ValidImageUrl);

            // Assert
            product.Name.Should().Be(ValidName);
            product.Description.Should().Be(ValidDescription);
            product.Price.Should().Be(ValidPrice);
            product.StockQuantity.Should().Be(ValidStock);
            product.CategoryId.Should().Be(_validCategoryId);
            product.ImageUrl.Should().Be(ValidImageUrl);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-0.01)]
        public void Constructor_WithNegativePrice_ShouldThrowDomainException(decimal invalidPrice)
        {
            // Act & Assert
            Action act = () => new Product(ValidName, ValidDescription, invalidPrice, ValidStock, _validCategoryId);
            act.Should().Throw<DomainException>().WithMessage("El precio no puede ser negativo.");
        }

        [Fact]
        public void Constructor_WithNegativeStock_ShouldThrowDomainException()
        {
            // Act & Assert
            Action act = () => new Product(ValidName, ValidDescription, ValidPrice, -1, _validCategoryId);
            act.Should().Throw<DomainException>().WithMessage("La cantidad en stock no puede ser negativa.");
        }

        [Fact]
        public void Update_WithValidParameters_ShouldUpdateProduct()
        {
            // Arrange
            var product = new Product("Old Name", "Old Desc", 10m, 10, Guid.NewGuid());
            var newCategoryId = Guid.NewGuid();

            // Act
            product.Update("New Name", "New Desc", 20m, 20, newCategoryId, "new.jpg");

            // Assert
            product.Name.Should().Be("New Name");
            product.Price.Should().Be(20m);
            product.StockQuantity.Should().Be(20);
            product.CategoryId.Should().Be(newCategoryId);
        }

        [Fact]
        public void AddStock_WithPositiveQuantity_ShouldIncreaseStock()
        {
            // Arrange
            var product = new Product(ValidName, ValidDescription, ValidPrice, 10, _validCategoryId);

            // Act
            product.AddStock(5);

            // Assert
            product.StockQuantity.Should().Be(15);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void AddStock_WithNonPositiveQuantity_ShouldThrowDomainException(int invalidQuantity)
        {
            // Arrange
            var product = new Product(ValidName, ValidDescription, ValidPrice, 10, _validCategoryId);

            // Act & Assert
            Action act = () => product.AddStock(invalidQuantity);
            act.Should().Throw<DomainException>().WithMessage("La cantidad para añadir al stock debe ser positiva.");
        }

        [Fact]
        public void RemoveStock_WithValidQuantity_ShouldDecreaseStock()
        {
            // Arrange
            var product = new Product(ValidName, ValidDescription, ValidPrice, 10, _validCategoryId);

            // Act
            product.RemoveStock(3);

            // Assert
            product.StockQuantity.Should().Be(7);
        }


        [Fact]
        public void RemoveStock_ExceedingAvailableStock_ShouldThrowDomainException()
        {
            // Arrange
            var product = new Product(ValidName, ValidDescription, ValidPrice, 10, _validCategoryId);

            // Act & Assert
            Action act = () => product.RemoveStock(11);
            act.Should().Throw<DomainException>().WithMessage("No hay suficiente stock para remover la cantidad solicitada.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void RemoveStock_WithNonPositiveQuantity_ShouldThrowDomainException(int invalidQuantity)
        {
            // Arrange
            var product = new Product(ValidName, ValidDescription, ValidPrice, 10, _validCategoryId);

            // Act & Assert
            Action act = () => product.RemoveStock(invalidQuantity);
            act.Should().Throw<DomainException>().WithMessage("La cantidad para remover del stock debe ser positiva.");
        }
    }
}
