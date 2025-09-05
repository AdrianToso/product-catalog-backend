using System.Linq;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Integration.Middleware
{
    public class CorrelationIdMiddlewareTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public CorrelationIdMiddlewareTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Middleware_WhenNoCorrelationIdHeaderIsProvided_ShouldGenerateNewId()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/products");

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();

            // Verifica que la cabecera de respuesta exista
            Assert.True(response.Headers.TryGetValues("X-Correlation-ID", out var correlationIds));

            // Verifica que el valor sea un GUID válido
            var correlationId = correlationIds.First();
            Assert.True(Guid.TryParse(correlationId, out _));
        }

        [Fact]
        public async Task Middleware_WhenCorrelationIdHeaderIsProvided_ShouldUseSameId()
        {
            // Arrange
            var providedCorrelationId = Guid.NewGuid().ToString();
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/products");
            request.Headers.Add("X-Correlation-ID", providedCorrelationId);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();

            // Verifica que la cabecera de respuesta exista
            Assert.True(response.Headers.TryGetValues("X-Correlation-ID", out var responseCorrelationIds));

            // Verifica que el ID en la respuesta sea el mismo que se envió
            var actualCorrelationId = responseCorrelationIds.First();
            Assert.Equal(providedCorrelationId, actualCorrelationId);
        }
    }
}
