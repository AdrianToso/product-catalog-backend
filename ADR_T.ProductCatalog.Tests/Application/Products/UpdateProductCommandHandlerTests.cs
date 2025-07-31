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
        var product = new Product("Original", "Desc", null);

        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetByIdWithCategoriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.ProductRepository).Returns(repo.Object);
        uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpdateProductCommandHandler(uow.Object);
        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(), // No importa
            Name = "Nuevo",
            Description = "Actualizado",
            ImageUrl = "img.png",
            CategoryIds = new List<Guid>()
        };

        // Act
        await handler.Handle(command, default);

        // Assert
        product.Name.Should().Be("Nuevo");
        product.Description.Should().Be("Actualizado");
        product.ImageUrl.Should().Be("img.png");
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
            CategoryIds = new List<Guid>()
        };

        // Act
        var act = async () => await handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Should_Update_Categories_When_Valid_CategoryIds_Provided()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category("Cat", "desc");
        typeof(Category).GetProperty("Id")!.SetValue(category, categoryId);

        var product = new Product("Producto X", "desc", null);

        var prodRepo = new Mock<IProductRepository>();
        prodRepo.Setup(r => r.GetByIdWithCategoriesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var catRepo = new Mock<ICategoryRepository>();
        catRepo.Setup(r => r.ListAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category> { category });

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
            CategoryIds = new List<Guid> { categoryId }
        };

        // Act
        await handler.Handle(command, default);

        // Assert
        product.Categories.Should().ContainSingle(c => c.Id == categoryId);
    }
}
