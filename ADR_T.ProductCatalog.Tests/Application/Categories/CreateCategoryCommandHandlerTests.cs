using ADR_T.ProductCatalog.Application.Features.Categories.Commands.CreateCategory;
using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ADR_T.ProductCatalog.Tests.Application.Categories;

public class CreateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_Category_When_Data_Is_Valid()
    {
        // Arrange
        var fakeRepo = new Mock<ICategoryRepository>();
        var fakeUoW = new Mock<IUnitOfWork>();

        fakeUoW.Setup(u => u.CategoryRepository).Returns(fakeRepo.Object);
        fakeUoW.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateCategoryCommandHandler(fakeUoW.Object);
        var command = new CreateCategoryCommand("Categoría A", "Descripción A");

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.Should().NotBeEmpty();
        fakeRepo.Verify(r => r.AddAsync(It.Is<Category>(c =>
            c.Name == command.Name &&
            c.Description == command.Description
        ), It.IsAny<CancellationToken>()), Times.Once);

        fakeUoW.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
