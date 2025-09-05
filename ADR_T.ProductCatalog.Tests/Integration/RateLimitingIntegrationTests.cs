using System.Net;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Integration
{
    // Usamos una colección para asegurar que este test se ejecute secuencialmente
    // y no interfiera con otros que puedan usar el mismo limitador.
    [Collection("Sequential")]
    public class RateLimitingIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public RateLimitingIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RateLimiter_WhenRequestsExceedLimit_ShouldReturnTooManyRequests()
        {
            // Arrange
            var requestUri = "/api/categories";
            // El límite en producción es 100. Haremos 105 peticiones para asegurar que lo superamos.
            var numberOfRequests = 105;
            HttpResponseMessage? lastResponse = null;

            // Act
            for (int i = 0; i < numberOfRequests; i++)
            {
                lastResponse = await _client.GetAsync(requestUri);
                // Si encontramos la respuesta 429, podemos detener el bucle antes.
                if (lastResponse.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    break;
                }
            }

            // Assert
            Assert.NotNull(lastResponse);
            // Verificamos que la última respuesta (o una de las últimas) fue bloqueada.
            Assert.Equal(HttpStatusCode.TooManyRequests, lastResponse.StatusCode);
        }
    }
}
