using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using FluentAssertions;

namespace ADR_T.ProductCatalog.Tests.Core.Domain.Entities;
public class AddressTests
{
    private readonly Guid _validCustomerId = Guid.NewGuid();
    private const string Street = "123 Main St";
    private const string City = "Anytown";
    private const string State = "CA";
    private const string PostalCode = "12345";
    private const string Country = "USA";

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateAddress()
    {
        // Act
        var address = new Address(_validCustomerId, Street, City, State, PostalCode, Country);

        // Assert
        address.CustomerId.Should().Be(_validCustomerId);
        address.Street.Should().Be(Street);
        address.City.Should().Be(City);
        address.State.Should().Be(State);
        address.PostalCode.Should().Be(PostalCode);
        address.Country.Should().Be(Country);
        address.IsPrimary.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithEmptyCustomerId_ShouldThrowDomainException()
    {
        // Act & Assert
        Action act = () => new Address(Guid.Empty, Street, City, State, PostalCode, Country);
        act.Should().Throw<DomainException>()
           .WithMessage("La dirección debe estar asociada a un cliente.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithInvalidStreet_ShouldThrowDomainException(string? invalidStreet)
    {
        // Act & Assert
        Action act = () => new Address(_validCustomerId, invalidStreet, City, State, PostalCode, Country);
        act.Should().Throw<DomainException>().WithMessage("La calle (Street) es requerida.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithInvalidCity_ShouldThrowDomainException(string? invalidCity)
    {
        // Act & Assert
        Action act = () => new Address(_validCustomerId, Street, invalidCity, State, PostalCode, Country);
        act.Should().Throw<DomainException>().WithMessage("La ciudad (City) es requerida.");
    }

    [Fact]
    public void SetAsPrimary_ShouldSetIsPrimaryToTrue()
    {
        // Arrange
        var address = new Address(_validCustomerId, Street, City, State, PostalCode, Country);

        // Act
        address.SetAsPrimary();

        // Assert
        address.IsPrimary.Should().BeTrue();
        address.FechacActualizacion.Should().NotBeNull();
    }

    [Fact]
    public void UnsetAsPrimary_ShouldSetIsPrimaryToFalse()
    {
        // Arrange
        var address = new Address(_validCustomerId, Street, City, State, PostalCode, Country, isPrimary: true);

        // Act
        address.UnsetAsPrimary();

        // Assert
        address.IsPrimary.Should().BeFalse();
        address.FechacActualizacion.Should().NotBeNull();
    }
}
