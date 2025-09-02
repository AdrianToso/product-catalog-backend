using ADR_T.ProductCatalog.Application.DTOs;
using ADR_T.ProductCatalog.Application.Features.Products.Queries.GetProductById;
using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using AutoMapper;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Application.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetProductByIdQueryHandler _handler;

        public GetProductByIdQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetProductByIdQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnProductDto_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            // Crear producto usando el constructor correcto (con categoryId en lugar del objeto Category)
            var product = new Product("Test Product", "Test Description", categoryId, null);

            var productDto = new ProductDto { Id = productId, Name = "Test Product" };

            // Usar reflexión para establecer el Id ya que la propiedad Id probablemente sea de solo lectura
            typeof(EntityBase).GetProperty("Id")?.SetValue(product, productId);

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetByIdWithCategoriesAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _mapperMock.Setup(m => m.Map<ProductDto>(product))
                .Returns(productDto);

            var query = new GetProductByIdQuery(productId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            _unitOfWorkMock.Verify(u => u.ProductRepository.GetByIdWithCategoriesAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
            _mapperMock.Verify(m => m.Map<ProductDto>(product), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _unitOfWorkMock.Setup(u => u.ProductRepository.GetByIdWithCategoriesAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product)null);

            var query = new GetProductByIdQuery(productId);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
            _unitOfWorkMock.Verify(u => u.ProductRepository.GetByIdWithCategoriesAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
