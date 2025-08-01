using ADR_T.ProductCatalog.Application.Features.Products.Commands.UpdateProduct;
using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ADR_T.ProductCatalog.Tests.Application.Products;

public class UpdateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Update_Product_When_Found()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var product = new Product("Original", "Desc", categoryId);

        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdWithCategoriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.ProductRepository).Returns(repo.Object);
        uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpdateProductCommandHandler(uow.Object);
        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            Name = "Nuevo",
            Description = "Actualizado",
            ImageUrl = "img.png",
            CategoryId = categoryId
        };

        // Act
        await handler.Handle(command, default);

        // Assert
        product.Name.Should().Be("Nuevo");
        product.Description.Should().Be("Actualizado");
        product.ImageUrl.Should().Be("img.png");
        product.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public async Task Handle_Should_Throw_NotFoundException_When_Product_Does_Not_Exist()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdWithCategoriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.ProductRepository).Returns(repo.Object);

        var handler = new UpdateProductCommandHandler(uow.Object);
        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            Name = "X",
            Description = "Y",
            ImageUrl = "img",
            CategoryId = Guid.NewGuid()
        };

        // Act
        var act = async () => await handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_Update_Category_When_Valid_CategoryId_Provided()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category("Cat", "desc");
        typeof(Category).GetProperty("Id")!.SetValue(category, categoryId);

        var product = new Product("Producto X", "desc", categoryId);

        var prodRepo = new Mock<IProductRepository>();
        prodRepo.Setup(r => r.GetByIdWithCategoriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var catRepo = new Mock<ICategoryRepository>();
        catRepo.Setup(r => r.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.ProductRepository).Returns(prodRepo.Object);
        uow.Setup(u => u.CategoryRepository).Returns(catRepo.Object);
        uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpdateProductCommandHandler(uow.Object);
        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            Name = "Actualizado",
            Description = "Nuevo desc",
            ImageUrl = "img.png",
            CategoryId = categoryId
        };

        // Act
        await handler.Handle(command, default);

        // Assert
        product.CategoryId.Should().Be(categoryId);
    }
}
