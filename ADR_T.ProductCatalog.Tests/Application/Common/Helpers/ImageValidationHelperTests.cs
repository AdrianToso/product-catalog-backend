using ADR_T.ProductCatalog.Application.Common.Helpers;
using Microsoft.AspNetCore.Http;
using Moq;

namespace ADR_T.ProductCatalog.Tests.Application.Common.Helpers
{
    public class ImageValidationHelperTests
    {
        [Fact]
        public void IsValidImage_ValidFile_ReturnsTrue()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.jpg");
            fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
            fileMock.Setup(f => f.Length).Returns(1024);

            // Act
            var result = ImageValidationHelper.IsValidImage(fileMock.Object, out var errorMessage);

            // Assert
            Assert.True(result);
            Assert.Null(errorMessage);
        }

        [Fact]
        public void IsValidImage_InvalidSize_ReturnsFalse()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.jpg");
            fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
            fileMock.Setup(f => f.Length).Returns(6 * 1024 * 1024); // 6MB - exceeds 5MB limit

            // Act
            var result = ImageValidationHelper.IsValidImage(fileMock.Object, out var errorMessage);

            // Assert
            Assert.False(result);
            Assert.Contains("excede el tamaño máximo", errorMessage);
        }

        [Fact]
        public void IsValidImage_EmptyFile_ReturnsFalse()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.jpg");
            fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
            fileMock.Setup(f => f.Length).Returns(0); // Empty file

            // Act
            var result = ImageValidationHelper.IsValidImage(fileMock.Object, out var errorMessage);

            // Assert
            Assert.False(result);
            Assert.Contains("vacío", errorMessage);
        }

        [Fact]
        public void IsValidImage_InvalidExtension_ReturnsFalse()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.txt"); // Invalid extension
            fileMock.Setup(f => f.ContentType).Returns("text/plain");
            fileMock.Setup(f => f.Length).Returns(1024);

            // Act
            var result = ImageValidationHelper.IsValidImage(fileMock.Object, out var errorMessage);

            // Assert
            Assert.False(result);
            Assert.Contains("Extensión no permitida", errorMessage);
        }

        [Fact]
        public void IsValidImage_InvalidMimeType_ReturnsFalse()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.jpg");
            fileMock.Setup(f => f.ContentType).Returns("text/plain"); // Invalid MIME type
            fileMock.Setup(f => f.Length).Returns(1024);

            // Act
            var result = ImageValidationHelper.IsValidImage(fileMock.Object, out var errorMessage);

            // Assert
            Assert.False(result);
            Assert.Contains("Tipo de archivo no permitido", errorMessage);
        }

        [Fact]
        public void IsValidImage_NullFile_ReturnsFalse()
        {
            // Act
            var result = ImageValidationHelper.IsValidImage(null, out var errorMessage);

            // Assert
            Assert.False(result);
            Assert.Contains("requerido", errorMessage);
        }
    }
}