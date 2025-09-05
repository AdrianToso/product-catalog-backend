using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Integration
{
    public class ResponseCachingIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ResponseCachingIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetCategories_ShouldReturnCorrectCacheHeaders()
        {
            // Arrange
            var requestUri = "/api/categories";

            // Act
            // Hacemos una SOLA petición.
            var response = await _client.GetAsync(requestUri);

            // Assert
            response.EnsureSuccessStatusCode();

            // Verificamos que la cabecera "Cache-Control" esté presente y configurada como esperamos.
            var cacheControlHeader = response.Headers.CacheControl;

            Assert.NotNull(cacheControlHeader);
            Assert.True(cacheControlHeader.Public);
            Assert.Equal(TimeSpan.FromSeconds(60), cacheControlHeader.MaxAge);
        }
    }
}
