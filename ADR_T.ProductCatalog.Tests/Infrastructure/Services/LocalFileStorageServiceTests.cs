using ADR_T.ProductCatalog.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Infrastructure.Services
{
    public class LocalFileStorageServiceTests : IDisposable
    {
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<LocalFileStorageService>> _mockLogger;
        private readonly string _tempTestDirectory;

        public LocalFileStorageServiceTests()
        {
            _tempTestDirectory = Path.Combine(Path.GetTempPath(), $"test_storage_{Guid.NewGuid()}");

            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockWebHostEnvironment.Setup(x => x.WebRootPath).Returns(_tempTestDirectory);

            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x["App:BaseUrl"]).Returns("https://example.com");

            _mockLogger = new Mock<ILogger<LocalFileStorageService>>();
        }

        public void Dispose()
        {
            // Clean up test directory
            if (Directory.Exists(_tempTestDirectory))
            {
                Directory.Delete(_tempTestDirectory, true);
            }
        }

        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var service = new LocalFileStorageService(
                _mockWebHostEnvironment.Object,
                _mockConfiguration.Object,
                _mockLogger.Object);

            // Assert - No exception should be thrown
            Assert.NotNull(service);
        }

        [Fact]
        public void Constructor_ShouldCreateDirectoryIfNotExists()
        {
            // Arrange
            var nonExistentPath = Path.Combine(_tempTestDirectory, "nonexistent");
            _mockWebHostEnvironment.Setup(x => x.WebRootPath).Returns(nonExistentPath);

            // Act
            var service = new LocalFileStorageService(
                _mockWebHostEnvironment.Object,
                _mockConfiguration.Object,
                _mockLogger.Object);

            // Assert
            Assert.True(Directory.Exists(nonExistentPath));
        }

        [Fact]
        public void Constructor_ShouldLogInitializationMessages()
        {
            // Act
            var service = new LocalFileStorageService(
                _mockWebHostEnvironment.Object,
                _mockConfiguration.Object,
                _mockLogger.Object);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Inicializando LocalFileStorageService")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task SaveFileAsync_WithValidFile_ShouldSaveFileAndReturnUrl()
        {
            // Arrange
            var service = new LocalFileStorageService(
                _mockWebHostEnvironment.Object,
                _mockConfiguration.Object,
                _mockLogger.Object);

            var testContent = new byte[] { 1, 2, 3, 4, 5 };
            var fileName = "test.jpg";
            using var stream = new MemoryStream(testContent);

            // Act
            var result = await service.SaveFileAsync(stream, fileName, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("https://example.com/images/products/", result);
            Assert.EndsWith(".jpg", result);

            // Verify file was actually created
            var files = Directory.GetFiles(Path.Combine(_tempTestDirectory, "images", "products"));
            Assert.Single(files);
        }

        [Fact]
        public async Task SaveFileAsync_WithEmptyStream_ShouldNotThrowException()
        {
            // Arrange
            var service = new LocalFileStorageService(
                _mockWebHostEnvironment.Object,
                _mockConfiguration.Object,
                _mockLogger.Object);

            using var emptyStream = new MemoryStream();
            var fileName = "test.jpg";

            // Act & Assert - No debería lanzar excepción
            var result = await service.SaveFileAsync(emptyStream, fileName, CancellationToken.None);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task SaveFileAsync_WithNullStream_ShouldThrowArgumentNullException()
        {
            // Arrange
            var service = new LocalFileStorageService(
                _mockWebHostEnvironment.Object,
                _mockConfiguration.Object,
                _mockLogger.Object);

            Stream nullStream = null;
            var fileName = "test.jpg";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                service.SaveFileAsync(nullStream, fileName, CancellationToken.None));
        }

        [Fact]
        public async Task SaveFileAsync_WithCancelledToken_ShouldThrowTaskCanceledException()
        {
            // Arrange
            var service = new LocalFileStorageService(
                _mockWebHostEnvironment.Object,
                _mockConfiguration.Object,
                _mockLogger.Object);

            var testContent = new byte[] { 1, 2, 3 };
            var fileName = "test.jpg";
            using var stream = new MemoryStream(testContent);
            var cancelledToken = new CancellationToken(canceled: true);

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() =>
                service.SaveFileAsync(stream, fileName, cancelledToken));
        }

        [Fact]
        public async Task SaveFileAsync_WithInvalidFileName_ShouldUseGuidExtension()
        {
            // Arrange
            var service = new LocalFileStorageService(
                _mockWebHostEnvironment.Object,
                _mockConfiguration.Object,
                _mockLogger.Object);

            var testContent = new byte[] { 1, 2, 3 };
            var fileName = "test"; // No extension
            using var stream = new MemoryStream(testContent);

            // Act
            var result = await service.SaveFileAsync(stream, fileName, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(".", result); // Should have extension from GUID
        }
    }
}
