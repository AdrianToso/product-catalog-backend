using ADR_T.ProductCatalog.Core.Domain.Entities;
using FluentAssertions;
using System;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Domain.Entities
{
    // Clase concreta para poder instanciar y probar la clase abstracta EntityBase
    public class ConcreteEntity : EntityBase
    {
        public ConcreteEntity() : base() { }
        public ConcreteEntity(Guid id) : base(id) { }
    }

    public class EntityBaseTests
    {
        [Fact]
        public void Constructor_Default_ShouldInitializePropertiesCorrectly()
        {
            // Arrange & Act
            var entity = new ConcreteEntity();

            // Assert
            entity.Id.Should().NotBe(Guid.Empty);
            entity.FechacCreacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            entity.FechacActualizacion.Should().BeNull();
            entity.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public void Constructor_WithGuid_ShouldSetIdCorrectly()
        {
            // Arrange
            var specificId = Guid.NewGuid();

            // Act
            var entity = new ConcreteEntity(specificId);

            // Assert
            entity.Id.Should().Be(specificId);
            // Las otras propiedades deberían seguir inicializándose correctamente
            entity.FechacCreacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void IsDeleted_Setter_ShouldUpdateValue()
        {
            // Arrange
            var entity = new ConcreteEntity();

            // Act
            entity.IsDeleted = true;

            // Assert
            entity.IsDeleted.Should().BeTrue();
        }
    }
}

