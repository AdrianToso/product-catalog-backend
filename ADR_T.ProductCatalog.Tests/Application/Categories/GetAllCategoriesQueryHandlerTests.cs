using ADR_T.ProductCatalog.Application.DTOs;
using ADR_T.ProductCatalog.Application.Features.Categories.Queries.GetAllCategories;
using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using AutoMapper;
using FluentAssertions;
using Moq;

namespace ADR_T.ProductCatalog.Tests.Application.Categories;

public class GetAllCategoriesQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_All_Categories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category("Cat1", "desc1"),
            new Category("Cat2", "desc2")
        };

        var repo = new Mock<ICategoryRepository>();
        var uow = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();

        repo.Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);
        uow.Setup(u => u.CategoryRepository).Returns(repo.Object);
        mapper.Setup(m => m.Map<List<CategoryDto>>(categories)).Returns(categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        }).ToList());

        var handler = new GetAllCategoriesQueryHandler(uow.Object, mapper.Object);

        // Act
        var result = await handler.Handle(new GetAllCategoriesQuery(), default);

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Cat1");
        result[1].Name.Should().Be("Cat2");
    }
}
