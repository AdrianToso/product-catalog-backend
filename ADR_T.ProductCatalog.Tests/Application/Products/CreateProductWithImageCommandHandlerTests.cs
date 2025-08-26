using ADR_T.ProductCatalog.Application.Features.Products.Commands.CreateProduct;
using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;

namespace ADR_T.ProductCatalog.Tests.Application.Products
{
    public class CreateProductWithImageCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IFileStorageService> _mockFileStorageService;
        private readonly CreateProductWithImageCommandHandler _handler;

        public CreateProductWithImageCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockFileStorageService = new Mock<IFileStorageService>();
            _handler = new CreateProductWithImageCommandHandler(_mockUnitOfWork.Object, _mockFileStorageService.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldCreateProductWithImage()
        {
            // Arrange
            var command = new CreateProductWithImageCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = Guid.NewGuid(),
                ImageFile = CreateTestFormFile("test.jpg", "image/jpeg", 1024)
            };

            _mockFileStorageService.Setup(s => s.SaveFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("https://localhost/images/test.jpg");

            _mockUnitOfWork.Setup(u => u.ProductRepository.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product p, CancellationToken _) => p);

            _mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
            _mockFileStorageService.Verify(s => s.SaveFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_NullImage_ShouldCreateProductWithoutImage()
        {
            // Arrange
            var command = new CreateProductWithImageCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = Guid.NewGuid(),
                ImageFile = null // No image
            };

            _mockUnitOfWork.Setup(u => u.ProductRepository.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product p, CancellationToken _) => p);

            _mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, result);
            _mockFileStorageService.Verify(s => s.SaveFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private IFormFile CreateTestFormFile(string fileName, string contentType, long length)
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.ContentType).Returns(contentType);
            fileMock.Setup(f => f.Length).Returns(length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[length]));
            return fileMock.Object;
        }
    }
}