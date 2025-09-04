using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ADR_T.ProductCatalog.Application.DTOs;
using ADR_T.ProductCatalog.Application.DTOs.Auth;
using Xunit;

namespace ADR_T.ProductCatalog.Tests.Integration;

public class CategoriesControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoriesControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        var loginPayload = new { Username = "admin_test", Password = "Admin123!" };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginPayload);
        response.EnsureSuccessStatusCode();
        var authResult = await response.Content.ReadFromJsonAsync<AuthResultDto>();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult?.Token);
    }

    [Fact]
    public async Task GetAllCategories_ShouldReturnOkAndCategoriesList()
    {
        var response = await _client.GetAsync("/api/categories");
        response.EnsureSuccessStatusCode();
        var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
        Assert.NotNull(categories);
        Assert.NotEmpty(categories);
    }

    [Fact]
    public async Task CreateCategory_WithValidDataAndAdminRole_ShouldReturnCreated()
    {
        await AuthenticateAsync();
        var categoryPayload = new { Name = $"Test Category {System.Guid.NewGuid()}", Description = "Test Description" };
        var response = await _client.PostAsJsonAsync("/api/categories", categoryPayload);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_WithoutAuth_ShouldReturnUnauthorized()
    {
        var categoryPayload = new { Name = "Unauthorized Category", Description = "This should fail" };
        var response = await _client.PostAsJsonAsync("/api/categories", categoryPayload);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
