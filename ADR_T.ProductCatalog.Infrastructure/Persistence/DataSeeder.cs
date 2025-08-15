using ADR_T.ProductCatalog.Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace ADR_T.ProductCatalog.Infrastructure.Persistence;

public class DataSeeder
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<DataSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            await _context.Database.EnsureCreatedAsync();

            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedCatalogAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrió un error durante el proceso de siembra de datos.");
            throw;
        }
    }

    private async Task SeedRolesAsync()
    {
        string[] roleNames = { "Admin", "Editor", "User" };

        foreach (var roleName in roleNames)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
                _logger.LogInformation("Rol '{RoleName}' creado.", roleName);
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        // Sembrar Usuario Administrador
        if (await _userManager.FindByNameAsync("admin") == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@example.com",
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(adminUser, new[] { "Admin", "Editor", "User" });
                _logger.LogInformation("Usuario 'admin' creado y asignado a roles.");
            }
        }

        // Sembrar Usuario Estándar (User)
        if (await _userManager.FindByNameAsync("user") == null)
        {
            var standardUser = new ApplicationUser
            {
                UserName = "user",
                Email = "user@example.com",
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(standardUser, "User123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(standardUser, "User");
                _logger.LogInformation("Usuario 'user' creado y asignado al rol 'User'.");
            }
        }
    }

    private async Task SeedCatalogAsync()
    {
        if (!_context.Categories.Any())
        {
            // --- INTEGRAR DATOS REALES DE CATEGORÍAS ---
            var categories = new List<Category>
            {
                new Category("Electrónica", "Dispositivos y gadgets electrónicos."),
                new Category("Libros", "Libros de ficción, no ficción y académicos."),
                new Category("Hogar", "Artículos para el hogar y decoración.")
            };
            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Siembra de {Count} categorías completada.", categories.Count);

            if (!_context.Products.Any())
            {
                // --- INTEGRAR DATOS REALES DE PRODUCTOS ---
                var electronicaCategory = categories.First(c => c.Name == "Electrónica");
                var librosCategory = categories.First(c => c.Name == "Libros");

                var products = new List<Product>
                {
                    new Product("Laptop Pro X", "Laptop de alto rendimiento para profesionales.", electronicaCategory.Id, "https://example.com/images/laptop.jpg"),
                    new Product("Smartphone Z", "El último modelo con cámara de alta resolución.", electronicaCategory.Id, "https://example.com/images/phone.jpg"),
                    new Product("El Legado del Viento", "Una novela de fantasía épica.", librosCategory.Id, "https://example.com/images/book1.jpg")
                };
                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Siembra de {Count} productos completada.", products.Count);
            }
        }
    }
}