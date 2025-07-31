using ADR_T.ProductCatalog.Application.Features.Categories.Commands.UpdateCategory;
using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ADR_T.ProductCatalog.Tests.Application.Categories;

public class UpdateCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Update_Category_When_Found()
    {
        // Arrange
        var category = new Category("Original", "Desc");
        var fakeRepo = new Mock<ICategoryRepository>();
        var fakeUoW = new Mock<IUnitOfWork>();

        fakeRepo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        fakeUoW.Setup(u => u.CategoryRepository).Returns(fakeRepo.Object);
        fakeUoW.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpdateCategoryCommandHandler(fakeUoW.Object);

        var command = new UpdateCategoryCommand(category.Id, "Actualizado", "Nueva desc");

        // Act
        await handler.Handle(command, default);

        // Assert
        category.Name.Should().Be("Actualizado");
        category.Description.Should().Be("Nueva desc");
        fakeRepo.Verify(r => r.UpdateAsync(category, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_NotFoundException_When_Category_Not_Exists()
    {
        // Arrange
        var fakeRepo = new Mock<ICategoryRepository>();
        var fakeUoW = new Mock<IUnitOfWork>();

        fakeRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        fakeUoW.Setup(u => u.CategoryRepository).Returns(fakeRepo.Object);

        var handler = new UpdateCategoryCommandHandler(fakeUoW.Object);

        var command = new UpdateCategoryCommand(Guid.NewGuid(), "No existe", "x");

        // Act
        var act = async () => await handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        fakeRepo.Verify(r => r.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
