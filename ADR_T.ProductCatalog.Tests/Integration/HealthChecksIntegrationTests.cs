using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Integration
{
    public class HealthChecksIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public HealthChecksIntegrationTests(CustomWebApplicationFactory factory)
        {
            // Solución Definitiva:
            // Usamos WithWebHostBuilder para crear una configuración específica para este test.
            _client = factory.WithWebHostBuilder(builder =>
            {
                // Usamos ConfigureTestServices, el método diseñado para reemplazar servicios en pruebas.
                builder.ConfigureTestServices(services =>
                {
                    // 1. Vaciamos la lista de todos los Health Checks registrados en Program.cs (SQL Server, FileStorage, etc.).
                    //    Esto evita que se ejecuten en el entorno de prueba.
                    services.Configure<HealthCheckServiceOptions>(options =>
                    {
                        options.Registrations.Clear();
                    });

                    // 2. Registramos únicamente los Health Checks que son compatibles con el entorno de pruebas.
                    services.AddHealthChecks()
                           .AddSqlite("DataSource=file::memory:?cache=shared", name: "Database");
                });
            }).CreateClient();
        }

        [Fact]
        public async Task HealthEndpoint_ShouldReturnHealthyStatus()
        {
            // Arrange
            var requestUri = "/health";

            // Act
            var response = await _client.GetAsync(requestUri);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Healthy", content);
        }
    }
}
