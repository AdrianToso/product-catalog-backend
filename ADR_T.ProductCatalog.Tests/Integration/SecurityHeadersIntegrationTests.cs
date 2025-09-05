using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ADR_T.ProductCatalog.Tests.Integration
{
    public class SecurityHeadersIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public SecurityHeadersIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task ApiResponse_ShouldContainSecurityHeaders()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/categories");

            // Act
            var response = await _client.SendAsync(request);

            _output.WriteLine("--- CABECERAS RECIBIDAS EN LA PRUEBA ---");
            foreach (var header in response.Headers)
            {
                _output.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }
            _output.WriteLine("--- FIN DE LAS CABECERAS ---");


            // Assert
            response.EnsureSuccessStatusCode();

            Assert.True(response.Headers.Contains("X-Content-Type-Options"), "La cabecera X-Content-Type-Options no se encontró.");
            Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").First());

            Assert.True(response.Headers.Contains("X-Frame-Options"), "La cabecera X-Frame-Options no se encontró.");
            Assert.Contains(response.Headers.GetValues("X-Frame-Options").First(), new[] { "DENY", "SAMEORIGIN" });

            Assert.True(response.Headers.Contains("Strict-Transport-Security"), "La cabecera Strict-Transport-Security no se encontró.");
            Assert.Contains("max-age", response.Headers.GetValues("Strict-Transport-Security").First());
        }
    }
}
