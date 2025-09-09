using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using FluentAssertions;
using System;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Core.Domain.Entities
{
    public class CustomerTests
    {
        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateCustomer()
        {
            // Arrange
            var userId = "auth|12345";
            var firstName = "John";
            var lastName = "Doe";

            // Act
            var customer = new Customer(userId, firstName, lastName);

            // Assert
            customer.UserId.Should().Be(userId);
            customer.FirstName.Should().Be(firstName);
            customer.LastName.Should().Be(lastName);
            customer.Orders.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidUserId_ShouldThrowDomainException(string? invalidUserId)
        {
            // Act & Assert
            Action act = () => new Customer(invalidUserId, "John", "Doe");
            act.Should().Throw<DomainException>()
               .WithMessage("Un Customer debe estar siempre asociado a un ApplicationUser.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidFirstName_ShouldThrowDomainException(string? invalidFirstName)
        {
            // Act & Assert
            Action act = () => new Customer("auth|12345", invalidFirstName, "Doe");
            act.Should().Throw<DomainException>()
               .WithMessage("El nombre (FirstName) no puede estar vacío.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidLastName_ShouldThrowDomainException(string? invalidLastName)
        {
            // Act & Assert
            Action act = () => new Customer("auth|12345", "John", invalidLastName);
            act.Should().Throw<DomainException>()
               .WithMessage("El apellido (LastName) no puede estar vacío.");
        }

        [Fact]
        public void UpdateName_WithValidParameters_ShouldUpdateName()
        {
            // Arrange
            var customer = new Customer("auth|12345", "John", "Doe");
            var newFirstName = "Jane";
            var newLastName = "Smith";

            // Act
            customer.UpdateName(newFirstName, newLastName);

            // Assert
            customer.FirstName.Should().Be(newFirstName);
            customer.LastName.Should().Be(newLastName);
            customer.FechacActualizacion.Should().NotBeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateName_WithInvalidFirstName_ShouldThrowDomainException(string? invalidFirstName)
        {
            // Arrange
            var customer = new Customer("auth|12345", "John", "Doe");

            // Act & Assert
            Action act = () => customer.UpdateName(invalidFirstName, "Smith");
            act.Should().Throw<DomainException>()
               .WithMessage("El nombre (FirstName) no puede estar vacío.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateName_WithInvalidLastName_ShouldThrowDomainException(string? invalidLastName)
        {
            // Arrange
            var customer = new Customer("auth|12345", "John", "Doe");

            // Act & Assert
            Action act = () => customer.UpdateName("Jane", invalidLastName);
            act.Should().Throw<DomainException>()
               .WithMessage("El apellido (LastName) no puede estar vacío.");
        }
    }
}
