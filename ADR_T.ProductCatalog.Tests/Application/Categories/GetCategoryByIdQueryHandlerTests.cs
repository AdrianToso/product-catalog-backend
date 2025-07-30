using ADR_T.ProductCatalog.Application.DTOs;
using ADR_T.ProductCatalog.Application.Features.Categories.Queries.GetCategoryById;
using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using AutoMapper;
using FluentAssertions;
using Moq;

namespace ADR_T.ProductCatalog.Tests.Application.Categories;

public class GetCategoryByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_CategoryDto_When_Found()
    {
        // Arrange
        var category = new Category("Test", "desc");
        var repo = new Mock<ICategoryRepository>();
        var uow = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();

        repo.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        uow.Setup(u => u.CategoryRepository).Returns(repo.Object);
        mapper.Setup(m => m.Map<CategoryDto>(category)).Returns(new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        });

        var handler = new GetCategoryByIdQueryHandler(uow.Object, mapper.Object);
        var query = new GetCategoryByIdQuery(category.Id);

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.Id.Should().Be(category.Id);
        result.Name.Should().Be(category.Name);
    }

    [Fact]
    public async Task Handle_Should_Throw_NotFoundException_When_Category_Not_Exists()
    {
        // Arrange
        var repo = new Mock<ICategoryRepository>();
        var uow = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();

        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);
        uow.Setup(u => u.CategoryRepository).Returns(repo.Object);

        var handler = new GetCategoryByIdQueryHandler(uow.Object, mapper.Object);
        var query = new GetCategoryByIdQuery(Guid.NewGuid());

        // Act
        var act = async () => await handler.Handle(query, default);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
