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
                ["Jwt:Key"] = "una-clave-secreta-muy-larga-y-segura-para-pruebas",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience"
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFileStorageService, LocalFileStorageService>();

            _connection = new SqliteConnection("DataSource=file::memory:?cache=shared");
            _connection.Open();
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));

            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddSignInManager<SignInManager<ApplicationUser>>();

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("una-clave-secreta-muy-larga-y-segura-para-pruebas")),
                    ValidateIssuer = true,
                    ValidIssuer = "TestIssuer",
                    ValidateAudience = true,
                    ValidAudience = "TestAudience",
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
                context.Database.EnsureCreated();

                var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
                var logger = sp.GetRequiredService<ILogger<CustomWebApplicationFactory>>();

                SeedAllForTestsAsync(roleManager, userManager, context, logger)
                    .GetAwaiter()
                    .GetResult();
            }

            _databaseInitialized = true;
        }
        return Task.CompletedTask;
    }

    public new async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        await base.DisposeAsync();
    }

    private async Task SeedAllForTestsAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, AppDbContext context, ILogger logger)
    {
        if (await roleManager.RoleExistsAsync("Admin")) return;

        logger.LogInformation("Sembrando base de datos de prueba...");
        string[] roleNames = { "Admin", "Editor", "User" };
        foreach (var roleName in roleNames) await roleManager.CreateAsync(new IdentityRole(roleName));

        var adminUser = new ApplicationUser { UserName = "admin_test", Email = "admin@test.com", EmailConfirmed = true };
        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded) await userManager.AddToRoleAsync(adminUser, "Admin");

        context.Categories.Add(new Category("Electrónica", "Dispositivos de prueba"));
        await context.SaveChangesAsync();
        logger.LogInformation("Base de datos de prueba sembrada.");
    }
}
