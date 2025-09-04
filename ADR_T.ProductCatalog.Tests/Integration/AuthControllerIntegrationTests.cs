using System.Net.Http.Json;
using ADR_T.ProductCatalog.Application.DTOs.Auth;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Integration
{
    public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_ValidNewUser_ReturnsAuthResultWithToken()
        {
            // Arrange
            var command = new
            {
                Username = $"testuser_{Guid.NewGuid()}",
                Email = $"test_{Guid.NewGuid()}@test.com",
                Password = "Test123!"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", command);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<AuthResultDto>();
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.Token));
        }

        [Fact]
        public async Task Login_WithSeededAdminCredentials_ReturnsToken()
        {
            // Arrange
            // Usuario sembrado desde CustomWebApplicationFactory
            var loginCommand = new
            {
                Username = "admin_test",
                Password = "Admin123!"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginCommand);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<AuthResultDto>();
            Assert.NotNull(result?.Token);
        }
    }
}
