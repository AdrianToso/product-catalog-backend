using ADR_T.ProductCatalog.Application.Features.Products.Commands.CreateProduct;
using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Application.Products
{
    public class CreateProductWithImageCommandValidatorTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CreateProductWithImageCommandValidator _validator;

        public CreateProductWithImageCommandValidatorTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            // Configurar el mock por defecto para CategoryRepository
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository
                .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, CancellationToken token) => new Category("Test Category"));

            _mockUnitOfWork.Setup(u => u.CategoryRepository).Returns(mockCategoryRepository.Object);

            _validator = new CreateProductWithImageCommandValidator(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task ValidCommand_ShouldNotHaveErrors()
        {
            // Arrange
            var command = new CreateProductWithImageCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = Guid.NewGuid(),
                ImageFile = CreateTestFormFile("test.jpg", "image/jpeg", 1024)
            };

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async Task InvalidImageType_ShouldHaveValidationError()
        {
            // Arrange
            var command = new CreateProductWithImageCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = Guid.NewGuid(),
                ImageFile = CreateTestFormFile("test.txt", "text/plain", 1024) // Invalid file type
            };

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ImageFile);
        }

        [Fact]
        public async Task NullImage_ShouldNotHaveValidationError()
        {
            // Arrange
            var command = new CreateProductWithImageCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = Guid.NewGuid(),
                ImageFile = null // Null file (opcional)
            };

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.ImageFile);
        }

        [Fact]
        public async Task LargeImage_ShouldHaveValidationError()
        {
            // Arrange
            var command = new CreateProductWithImageCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = Guid.NewGuid(),
                ImageFile = CreateTestFormFile("test.jpg", "image/jpeg", 6 * 1024 * 1024) // 6MB - exceeds limit
            };

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ImageFile);
        }

        [Fact]
        public async Task EmptyName_ShouldHaveValidationError()
        {
            // Arrange
            var command = new CreateProductWithImageCommand
            {
                Name = "", // Empty name
                Description = "Test Description",
                CategoryId = Guid.NewGuid(),
                ImageFile = CreateTestFormFile("test.jpg", "image/jpeg", 1024)
            };

            // Act
            var result = await _validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public async Task NonExistingCategory_ShouldHaveValidationError()
        {
            // Arrange
            var command = new CreateProductWithImageCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = Guid.NewGuid(), // Non-existing category
                ImageFile = CreateTestFormFile("test.jpg", "image/jpeg", 1024)
            };

            // Configurar mock para simular categoría inexistente
            var mockCategoryRepository = new Mock<ICategoryRepository>();
            mockCategoryRepository
                .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category)null);

            _mockUnitOfWork.Setup(u => u.CategoryRepository).Returns(mockCategoryRepository.Object);

            var validator = new CreateProductWithImageCommandValidator(_mockUnitOfWork.Object);

            // Act
            var result = await validator.TestValidateAsync(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CategoryId);
        }

        private IFormFile CreateTestFormFile(string fileName, string contentType, long length)
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.ContentType).Returns(contentType);
            fileMock.Setup(f => f.Length).Returns(length);
            return fileMock.Object;
        }
    }
}