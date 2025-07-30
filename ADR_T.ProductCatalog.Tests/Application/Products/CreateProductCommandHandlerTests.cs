using ADR_T.ProductCatalog.Application.Features.Products.Commands.CreateProduct;
using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ADR_T.ProductCatalog.Tests.Application.Products;

public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_Product_Without_Categories()
    {
        // Arrange
        var repo = new Mock<IProductRepository>();
        var catRepo = new Mock<ICategoryRepository>();
        var uow = new Mock<IUnitOfWork>();

        uow.Setup(u => u.ProductRepository).Returns(repo.Object);
        uow.Setup(u => u.CategoryRepository).Returns(catRepo.Object);
        uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateProductCommandHandler(uow.Object);

        var command = new CreateProductCommand("Producto A", "Desc", null, new List<Guid>());

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.Should().NotBeEmpty();
        repo.Verify(r => r.AddAsync(It.Is<Product>(p =>
            p.Name == command.Name &&
            p.Description == command.Description &&
            p.ImageUrl == command.ImageUrl &&
            p.Categories.Count == 0
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Associate_Categories_When_Valid_CategoryIds_Provided()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category("Cat", "desc");
        typeof(Category).GetProperty("Id")!.SetValue(category, categoryId);

        var catRepo = new Mock<ICategoryRepository>();
        catRepo.Setup(r => r.ListAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category> { category });

        var prodRepo = new Mock<IProductRepository>();
        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.ProductRepository).Returns(prodRepo.Object);
        uow.Setup(u => u.CategoryRepository).Returns(catRepo.Object);
        uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateProductCommandHandler(uow.Object);

        var command = new CreateProductCommand("Producto B", "Desc", "img.jpg", new List<Guid> { categoryId });

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.Should().NotBeEmpty();
        prodRepo.Verify(r => r.AddAsync(It.Is<Product>(p =>
            p.Categories.Count == 1 &&
            p.Categories.First().Id == categoryId
        ), It.IsAny<CancellationToken>()), Times.Once);
    }
}
