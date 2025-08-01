using ADR_T.ProductCatalog.Application.Features.Products.Commands.CreateProduct;
using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ADR_T.ProductCatalog.Tests.Application.Products;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly CreateProductCommandHandler _handler;
    private readonly CreateProductCommandValidator _validator;

    public CreateProductCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();

        _mockUnitOfWork.Setup(uow => uow.ProductRepository).Returns(_mockProductRepository.Object);
        _mockUnitOfWork.Setup(uow => uow.CategoryRepository).Returns(_mockCategoryRepository.Object);

        _handler = new CreateProductCommandHandler(_mockUnitOfWork.Object);
        _validator = new CreateProductCommandValidator(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateProductAndReturnId()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category("Test Category");

        _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(category);

        var command = new CreateProductCommand("Test Product", "Test Description", "test.jpg", categoryId);

        _mockProductRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken _) => p);

        _mockUnitOfWork.Setup(uow => uow.CommitAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        _mockProductRepository.Verify(repo => repo.AddAsync(It.Is<Product>(p =>
            p.Name == command.Name &&
            p.Description == command.Description &&
            p.ImageUrl == command.ImageUrl &&
            p.CategoryId == command.CategoryId
        ), It.IsAny<CancellationToken>()), Times.Once);

        _mockUnitOfWork.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidCategoryId_ShouldThrowValidationException()
    {
        // Arrange
        var invalidCategoryId = Guid.NewGuid();
        _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(invalidCategoryId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync((Category?)null);

        var command = new CreateProductCommand("Test Product", "Test Description", "test.jpg", invalidCategoryId);

        // Act
        var validationResult = await _validator.ValidateAsync(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e =>
            e.PropertyName == nameof(command.CategoryId) &&
            e.ErrorMessage == "La categoría especificada no existe.");
    }

    [Fact]
    public async Task Handle_EmptyName_ShouldThrowValidationException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category("Test Category");

        _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(category);

        var command = new CreateProductCommand("", "Test Description", "test.jpg", categoryId);

        // Act
        var validationResult = await _validator.ValidateAsync(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e =>
            e.PropertyName == nameof(command.Name) &&
            e.ErrorMessage == "El nombre es requerido.");
    }

    [Fact]
    public async Task Handle_NameTooLong_ShouldThrowValidationException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category("Test Category");

        _mockCategoryRepository.Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(category);

        var longName = new string('a', 101);
        var command = new CreateProductCommand(longName, "Test Description", "test.jpg", categoryId);

        // Act
        var validationResult = await _validator.ValidateAsync(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e =>
            e.PropertyName == nameof(command.Name) &&
            e.ErrorMessage == "El nombre no debe exceder los 100 caracteres.");
    }
}
