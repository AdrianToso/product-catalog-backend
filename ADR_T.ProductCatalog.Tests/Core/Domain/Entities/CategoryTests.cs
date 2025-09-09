using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using FluentAssertions;
using System;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Core.Domain.Entities
{
    public class CategoryTests
    {
        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateCategory()
        {
            // Act
            var category = new Category("Electrónica", "Dispositivos electrónicos");

            // Assert
            category.Name.Should().Be("Electrónica");
            category.Description.Should().Be("Dispositivos electrónicos");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidName_ShouldThrowDomainException(string? invalidName)
        {
            // Act & Assert
            Action act = () => new Category(invalidName, "Una descripción");
            act.Should().Throw<DomainException>()
               .WithMessage("El nombre de la categoría no puede ser nulo o vacío.");
        }

        [Fact]
        public void Constructor_WithNameTooLong_ShouldThrowDomainException()
        {
            // Arrange
            var longName = new string('a', 101);

            // Act & Assert
            Action act = () => new Category(longName, "Una descripción");
            act.Should().Throw<DomainException>()
               .WithMessage("El nombre de la categoría no puede exceder los 100 caracteres.");
        }

        [Fact]
        public void Update_WithValidParameters_ShouldUpdateCategory()
        {
            // Arrange
            var category = new Category("Nombre Viejo", "Descripción Vieja");

            // Act
            category.Update("Nombre Nuevo", "Descripción Nueva");

            // Assert
            category.Name.Should().Be("Nombre Nuevo");
            category.Description.Should().Be("Descripción Nueva");
            category.FechacActualizacion.Should().NotBeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_WithInvalidName_ShouldThrowDomainException(string? invalidName)
        {
            // Arrange
            var category = new Category("Nombre Viejo", "Descripción Vieja");

            // Act & Assert
            Action act = () => category.Update(invalidName, "Descripción Nueva");
            act.Should().Throw<DomainException>()
               .WithMessage("El nombre de la categoría no puede ser nulo o vacío.");
        }
    }
}
