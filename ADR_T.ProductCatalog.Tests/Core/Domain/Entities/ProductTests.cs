using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Core.Domain.Entities
{
    public class ProductTests
    {
        private readonly Guid _validCategoryId = Guid.NewGuid();
        private const string ValidName = "Test Product";
        private const string ValidDescription = "Test Description";
        private const string ValidImageUrl = "https://example.com/image.jpg";

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateProduct()
        {
            // Act
            var product = new Product(ValidName, ValidDescription, _validCategoryId, ValidImageUrl);

            // Assert
            product.Name.Should().Be(ValidName);
            product.Description.Should().Be(ValidDescription);
            product.CategoryId.Should().Be(_validCategoryId);
            product.ImageUrl.Should().Be(ValidImageUrl);
            product.FechacCreacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            product.FechacActualizacion.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithNullImageUrl_ShouldCreateProduct()
        {
            // Act
            var product = new Product(ValidName, ValidDescription, _validCategoryId, null);

            // Assert
            product.ImageUrl.Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidName_ShouldThrowDomainException(string? invalidName)
        {
            // Act & Assert
            Action act = () => new Product(invalidName, ValidDescription, _validCategoryId);
            act.Should().Throw<DomainException>()
                .WithMessage("El nombre del producto no puede ser nulo o vacío.");
        }

        [Fact]
        public void Constructor_WithNameExceeding200Characters_ShouldThrowDomainException()
        {
            // Arrange
            var longName = new string('a', 201);

            // Act & Assert
            Action act = () => new Product(longName, ValidDescription, _validCategoryId);
            act.Should().Throw<DomainException>()
                .WithMessage("El nombre del producto no puede exceder los 200 caracteres.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidDescription_ShouldThrowDomainException(string? invalidDescription)
        {
            // Act & Assert
            Action act = () => new Product(ValidName, invalidDescription, _validCategoryId);
            act.Should().Throw<DomainException>()
                .WithMessage("La descripción del producto no puede ser nula o vacía.");
        }

        [Fact]
        public void Constructor_WithEmptyCategoryId_ShouldThrowDomainException()
        {
            // Act & Assert
            Action act = () => new Product(ValidName, ValidDescription, Guid.Empty);
            act.Should().Throw<DomainException>()
                .WithMessage("La categoría del producto no puede ser un GUID vacío.");
        }

        [Fact]
        public void Update_WithValidParameters_ShouldUpdateProduct()
        {
            // Arrange
            var product = new Product("Old Name", "Old Description", Guid.NewGuid());
            var newName = "Updated Product";
            var newDescription = "Updated Description";
            var newCategoryId = Guid.NewGuid();
            var newImageUrl = "https://example.com/new-image.jpg";

            // Act
            product.Update(newName, newDescription, newCategoryId, newImageUrl);

            // Assert
            product.Name.Should().Be(newName);
            product.Description.Should().Be(newDescription);
            product.CategoryId.Should().Be(newCategoryId);
            product.ImageUrl.Should().Be(newImageUrl);
            product.FechacActualizacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Update_WithNullImageUrl_ShouldUpdateProduct()
        {
            // Arrange
            var product = new Product(ValidName, ValidDescription, _validCategoryId, ValidImageUrl);

            // Act
            product.Update("New Name", "New Description", _validCategoryId, null);

            // Assert
            product.ImageUrl.Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_WithInvalidName_ShouldThrowDomainException(string? invalidName)
        {
            // Arrange
            var product = new Product(ValidName, ValidDescription, _validCategoryId);

            // Act & Assert
            Action act = () => product.Update(invalidName, ValidDescription, _validCategoryId);
            act.Should().Throw<DomainException>()
                .WithMessage("El nombre del producto no puede ser nulo o vacío.");
        }

        [Fact]
        public void Update_WithNameExceeding200Characters_ShouldThrowDomainException()
        {
            // Arrange
            var product = new Product(ValidName, ValidDescription, _validCategoryId);
            var longName = new string('a', 201);

            // Act & Assert
            Action act = () => product.Update(longName, ValidDescription, _validCategoryId);
            act.Should().Throw<DomainException>()
                .WithMessage("El nombre del producto no puede exceder los 200 caracteres.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_WithInvalidDescription_ShouldThrowDomainException(string? invalidDescription)
        {
            // Arrange
            var product = new Product(ValidName, ValidDescription, _validCategoryId);

            // Act & Assert
            Action act = () => product.Update(ValidName, invalidDescription, _validCategoryId);
            act.Should().Throw<DomainException>()
                .WithMessage("La descripción del producto no puede ser nula o vacía.");
        }

        [Fact]
        public void Update_WithEmptyCategoryId_ShouldThrowDomainException()
        {
            // Arrange
            var product = new Product(ValidName, ValidDescription, _validCategoryId);

            // Act & Assert
            Action act = () => product.Update(ValidName, ValidDescription, Guid.Empty);
            act.Should().Throw<DomainException>()
                .WithMessage("La categoría del producto no puede ser un GUID vacío.");
        }

        [Fact]
        public void SetImageUrl_WithValidUrl_ShouldUpdateImageUrl()
        {
            // Arrange
            var product = new Product(ValidName, ValidDescription, _validCategoryId);
            var newImageUrl = "https://example.com/new-image.jpg";

            // Act
            product.SetImageUrl(newImageUrl);

            // Assert
            product.ImageUrl.Should().Be(newImageUrl);
            product.FechacActualizacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void SetImageUrl_WithInvalidUrl_ShouldThrowDomainException(string? invalidUrl)
        {
            // Arrange
            var product = new Product(ValidName, ValidDescription, _validCategoryId);

            // Act & Assert
            Action act = () => product.SetImageUrl(invalidUrl);
            act.Should().Throw<DomainException>()
                .WithMessage("La URL de la imagen no puede ser vacía.");
        }

        [Fact]
        public void SetImageUrl_ShouldUpdateTimestamp()
        {
            // Arrange
            var product = new Product(ValidName, ValidDescription, _validCategoryId);
            var initialTimestamp = product.FechacActualizacion;

            // Act
            product.SetImageUrl(ValidImageUrl);

            // Assert
            product.FechacActualizacion.Should().NotBe(initialTimestamp);
            product.FechacActualizacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }
    }
}
