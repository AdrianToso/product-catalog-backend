using ADR_T.ProductCatalog.Application.DTOs;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Application.DTOs
{
    public class ProductDtoTests
    {
        [Fact]
        public void ProductDto_ShouldSetAndGetPropertiesCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var categoryDto = new CategoryDto { Id = Guid.NewGuid(), Name = "Test Category" };

            // Act
            var productDto = new ProductDto
            {
                Id = id,
                Name = "Test Product",
                Description = "Test Description",
                ImageUrl = "test.jpg",
                Category = categoryDto
            };

            // Assert
            Assert.Equal(id, productDto.Id);
            Assert.Equal("Test Product", productDto.Name);
            Assert.Equal("Test Description", productDto.Description);
            Assert.Equal("test.jpg", productDto.ImageUrl);
            Assert.Equal(categoryDto, productDto.Category);
        }

        [Fact]
        public void ProductDto_ShouldHandleNullImageUrl()
        {
            // Arrange & Act
            var productDto = new ProductDto
            {
                Id = Guid.NewGuid(),
                Name = "Test Product",
                Description = "Test Description",
                ImageUrl = null,
                Category = new CategoryDto { Id = Guid.NewGuid(), Name = "Test Category" }
            };

            // Assert
            Assert.Null(productDto.ImageUrl);
        }

        [Fact]
        public void ProductDto_RecordsShouldHaveValueEquality()
        {
            // Arrange
            var id = Guid.NewGuid();
            var categoryDto = new CategoryDto { Id = Guid.NewGuid(), Name = "Test Category" };

            var productDto1 = new ProductDto
            {
                Id = id,
                Name = "Test Product",
                Description = "Test Description",
                ImageUrl = "test.jpg",
                Category = categoryDto
            };

            var productDto2 = new ProductDto
            {
                Id = id,
                Name = "Test Product",
                Description = "Test Description",
                ImageUrl = "test.jpg",
                Category = categoryDto
            };

            // Act & Assert
            Assert.Equal(productDto1, productDto2);
            Assert.True(productDto1 == productDto2);
        }
    }
}
