using ADR_T.ProductCatalog.Application.Features.Categories.Commands.DeleteCategory;
using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ADR_T.ProductCatalog.Tests.Application.Categories;

public class DeleteCategoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Delete_Category_When_Found()
    {
        // Arrange
        var category = new Category("A borrar", "desc");

        var repo = new Mock<ICategoryRepository>();
        var uow = new Mock<IUnitOfWork>();

        repo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        uow.Setup(u => u.CategoryRepository).Returns(repo.Object);
        uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new DeleteCategoryCommandHandler(uow.Object);
        var command = new DeleteCategoryCommand(category.Id);

        // Act
        await handler.Handle(command, default);

        // Assert
        repo.Verify(r => r.DeleteAsync(category, It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_NotFoundException_When_Category_Does_Not_Exist()
    {
        // Arrange
        var repo = new Mock<ICategoryRepository>();
        var uow = new Mock<IUnitOfWork>();

        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        uow.Setup(u => u.CategoryRepository).Returns(repo.Object);

        var handler = new DeleteCategoryCommandHandler(uow.Object);
        var command = new DeleteCategoryCommand(Guid.NewGuid());

        // Act
        var act = async () => await handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        repo.Verify(r => r.DeleteAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
