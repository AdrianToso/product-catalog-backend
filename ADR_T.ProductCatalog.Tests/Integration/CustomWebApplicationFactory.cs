using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using ADR_T.ProductCatalog.Infrastructure.Persistence;
using ADR_T.ProductCatalog.Infrastructure.Repositories;
using ADR_T.ProductCatalog.Infrastructure.Services;
using ADR_T.ProductCatalog.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Data.Common;
using System.Text;

namespace ADR_T.ProductCatalog.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<ProgramPublicForTesting>, IAsyncLifetime
{
    // Objetos para controlar la inicialización única y segura de la BD
    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    private DbConnection _connection = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, conf) =>
        {
            conf.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=product_catalog_test;Trusted_Connection=false;MultipleActiveResultSets=true",

                ["Jwt:Key"] = "test_jwt_secret_key_min_32_chars_1234567890",
                ["Jwt:Issuer"] = "product-catalog-test",
                ["Jwt:Audience"] = "product-catalog-test",
                ["Jwt:ExpireMinutes"] = "60",

                ["API_KEY"] = "test_api_key_12345",

                ["ASPNETCORE_ENVIRONMENT"] = "Testing",
                ["ALLOWED_ORIGINS"] = "http://localhost:3000",
                ["RATE_LIMITING__PERMIT_LIMIT"] = "100",
                ["RATE_LIMITING__WINDOW"] = "1",
                ["FILE_STORAGE__PATH"] = "./storage-test",
                ["LOGGING__LEVEL"] = "Information"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext configuration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            // Register repositories and services
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFileStorageService, LocalFileStorageService>();

            _connection = new SqliteConnection("DataSource=file::memory:?cache=shared");
            _connection.Open();
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(_connection);
                options.EnableSensitiveDataLogging();
            });

            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddSignInManager<SignInManager<ApplicationUser>>();

            // JWT Authentication configuration
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test_jwt_secret_key_min_32_chars_1234567890")),
                    ValidateIssuer = true,
                    ValidIssuer = "product-catalog-test",
                    ValidateAudience = true,
                    ValidAudience = "product-catalog-test",
                    ValidateLifetime = false
                };
            });

            services.AddAuthorization();
        });
    }

    public Task InitializeAsync()
    {
        lock (_lock)
        {
            if (_databaseInitialized)
            {
                return Task.CompletedTask;
            }

            using (var scope = Services.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var context = sp.GetRequiredService<AppDbContext>();

                try
                {
                    context.Database.EnsureCreated();

                    var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
                    var logger = sp.GetRequiredService<ILogger<CustomWebApplicationFactory>>();

                    SeedAllForTestsAsync(roleManager, userManager, context, logger)
                        .GetAwaiter()
                        .GetResult();
                }
                catch (Exception ex)
                {
                    var logger = sp.GetRequiredService<ILogger<CustomWebApplicationFactory>>();
                    logger.LogError(ex, "An error occurred seeding the test database");
                    throw;
                }
            }

            _databaseInitialized = true;
        }
        return Task.CompletedTask;
    }

    public new async Task DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }
        await base.DisposeAsync();
    }

    private async Task SeedAllForTestsAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, AppDbContext context, ILogger logger)
    {
        if (await roleManager.RoleExistsAsync("Admin"))
            return;

        logger.LogInformation("Sembrando base de datos de prueba...");
        string[] roleNames = { "Admin", "Editor", "User" };
        foreach (var roleName in roleNames)
            await roleManager.CreateAsync(new IdentityRole(roleName));

        var adminUser = new ApplicationUser
        {
            UserName = "admin_test",
            Email = "admin@test.com",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Admin");

        context.Categories.Add(new Category("Electrónica", "Dispositivos de prueba"));
        context.Categories.Add(new Category("Ropa", "Prendas de vestir de prueba"));

        await context.SaveChangesAsync();
        logger.LogInformation("Base de datos de prueba sembrada.");
    }
}
