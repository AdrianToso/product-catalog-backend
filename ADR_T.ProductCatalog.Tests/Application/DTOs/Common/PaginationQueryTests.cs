using ADR_T.ProductCatalog.Application.DTOs.Common;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Application.DTOs.Common
{
    public class PaginationQueryTests
    {
        // Clase concreta para testing
        private class TestPaginationQuery : PaginationQuery { }

        [Fact]
        public void DefaultValues_ShouldBeCorrect()
        {
            // Arrange & Act
            var query = new TestPaginationQuery();

            // Assert
            Assert.Equal(1, query.PageNumber);
            Assert.Equal(10, query.PageSize);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(5, 5)]
        [InlineData(0, 1)]    // Valor menor a 1 debería ser 1
        [InlineData(-1, 1)]   // Valor negativo debería ser 1
        [InlineData(100, 100)] // Valor positivo debería mantenerse
        public void PageNumber_Setter_ShouldValidateCorrectly(int input, int expected)
        {
            // Arrange
            var query = new TestPaginationQuery();

            // Act
            query.PageNumber = input;

            // Assert
            Assert.Equal(expected, query.PageNumber);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(25, 25)]
        [InlineData(50, 50)]   // Límite máximo
        [InlineData(51, 50)]   // Mayor al máximo debería ser 50
        [InlineData(0, 50)]    // Menor a 1 debería ser 50
        [InlineData(-1, 50)]   // Negativo debería ser 50
        public void PageSize_Setter_ShouldValidateCorrectly(int input, int expected)
        {
            // Arrange
            var query = new TestPaginationQuery();

            // Act
            query.PageSize = input;

            // Assert
            Assert.Equal(expected, query.PageSize);
        }

        [Fact]
        public void MaxPageSize_ShouldBe50()
        {
            // Arrange
            var query = new TestPaginationQuery();

            // Act - Intentar establecer un valor mayor al máximo
            query.PageSize = 100;

            // Assert - Debería limitarse a 50
            Assert.Equal(50, query.PageSize);
        }
    }
}
