using ADR_T.ProductCatalog.Application.Features.Products.Commands.UpdateProductImage;
using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
namespace ADR_T.ProductCatalog.Tests.Application.Products;
public class UpdateProductImageCommandHandlerTests
{
    [Fact]
    public async Task Should_Update_Product_ImageUrl_When_File_Is_Valid()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product("Test", "Description", Guid.NewGuid());

        var mockUow = new Mock<IUnitOfWork>();
        mockUow.Setup(u => u.ProductRepository.GetByIdAsync(productId, default))
               .ReturnsAsync(product);

        var mockFileStorage = new Mock<IFileStorageService>();
        mockFileStorage.Setup(s => s.SaveFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), default))
                       .ReturnsAsync("https://fakeurl.com/image.jpg");

        var handler = new UpdateProductImageCommandHandler(mockUow.Object, mockFileStorage.Object);

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[10]));
        fileMock.Setup(f => f.FileName).Returns("test.jpg");
        fileMock.Setup(f => f.Length).Returns(10);
        fileMock.Setup(f => f.ContentType).Returns("image/jpeg");

        var command = new UpdateProductImageCommand(productId, fileMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.Equal("https://fakeurl.com/image.jpg", result);
        Assert.Equal("https://fakeurl.com/image.jpg", product.ImageUrl);
        mockUow.Verify(u => u.CommitAsync(default), Times.Once);
    }
}
