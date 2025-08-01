using ADR_T.ProductCatalog.Application.Features.Products.Commands.DeleteProduct;
using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ADR_T.ProductCatalog.Tests.Application.Products;

public class DeleteProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Delete_Product_When_Found()
    {
        // Arrange
        var product = new Product("Producto A", "desc", Guid.NewGuid());

        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.ProductRepository).Returns(repo.Object);
        uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new DeleteProductCommandHandler(uow.Object);

        var command = new DeleteProductCommand(Guid.NewGuid());

        // Act
        await handler.Handle(command, default);

        // Assert
        repo.Verify(r => r.DeleteAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_NotFoundException_When_Product_Does_Not_Exist()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.ProductRepository).Returns(repo.Object);

        var handler = new DeleteProductCommandHandler(uow.Object);
        var command = new DeleteProductCommand(Guid.NewGuid());

        // Act
        var act = async () => await handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}

