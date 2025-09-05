using ADR_T.ProductCatalog.Application;
using ADR_T.ProductCatalog.Infrastructure;
using ADR_T.ProductCatalog.Infrastructure.Persistence;
using ADR_T.ProductCatalog.WebApi.Filters;
using ADR_T.ProductCatalog.WebApi.HealthChecks;
using ADR_T.ProductCatalog.WebApi.Middleware;
using ADR_T.ProductCatalog.WebAPI.Middleware;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .CreateLogger();

builder.Host.UseSerilog();

// Registra los servicios de Compresión de Respuesta
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

// Configurar límites de Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 52428800; // 50 MB
});

// Configurar límites de formularios
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 52428800; // 50 MB
    options.MultipartHeadersCountLimit = 100;
    options.MultipartHeadersLengthLimit = 16384;
    options.ValueLengthLimit = 134217728; // 128 MB
    options.BufferBodyLengthLimit = 134217728; // 128 MB
});
// Registra los servicios del Límite de Tasa (Rate Limiter)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 0;
    });

    // Código de respuesta cuando se alcanza el límite
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddResponseCaching();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
});

// Implementación de Health Checks
builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "Database",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "dependencies" })
    .AddCheck<FileStorageHealthCheck>(
        "File Storage",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "dependencies" });

// Configura los servicios para el Dashboard de Health Checks UI
builder.Services.AddHealthChecksUI(setup =>
{
    // Indica a la UI qué endpoint debe consultar para obtener el estado de salud
    setup.AddHealthCheckEndpoint("API Health", "/health");
    setup.SetEvaluationTimeInSeconds(15); // Frecuencia de sondeo
})
.AddInMemoryStorage();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product Catalog API",
        Version = "v1",
        Description = "API para gestión de productos y categorías",
        Contact = new OpenApiContact
        {
            Name = "Adrián Toso",
            Url = new Uri("https://www.linkedin.com/in/adrian-toso-24b96419/")
        }
    });

    options.OperationFilter<SwaggerFileOperationFilter>();

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor, ingrese 'Bearer' seguido de un espacio y el token JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddApplicationServices();

if (builder.Environment.IsEnvironment("Testing") == false)
{
    builder.Services.AddInfrastructureServices(builder.Configuration);
}

var myCorsPolicy = "MyCorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myCorsPolicy,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Esto asegura que las cabeceras como HSTS se añadan durante los tests.
if (app.Environment.IsEnvironment("Testing"))
{
    app.Use((context, next) =>
    {
        context.Request.Scheme = "https";
        return next();
    });
}
// Siembra de datos inicial
if (app.Environment.IsEnvironment("Testing") == false)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Iniciando siembra de datos...");
        var seeder = services.GetRequiredService<DataSeeder>();
        await seeder.SeedAsync();
        logger.LogInformation("Siembra de datos finalizada.");
    }
}
app.UseResponseCompression();
// Middlewares
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Catalog API v1");
        options.RoutePrefix = "swagger";
    });
}

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
{
    app.UseMiddleware<ExceptionHandlingMiddleware>();
}

app.UseHttpsRedirection();
app.UseSecurityHeaders(policy =>
    policy.AddDefaultSecurityHeaders()
          .AddStrictTransportSecurity(60 * 60 * 24 * 365, true, false)
          .AddContentSecurityPolicy(builder =>
          {
              builder.AddBlockAllMixedContent();
              builder.AddDefaultSrc().Self();
              builder.AddImgSrc().Self().From("data:");
              builder.AddStyleSrc().Self().UnsafeInline();
              builder.AddScriptSrc().Self().UnsafeInline();
          })
);
app.UseStaticFiles();
app.UseRateLimiter();
app.UseResponseCaching();
app.UseCors(myCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

// Mapeo de endpoints de Health Checks
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/healthchecks-ui"; // Define la ruta del dashboard
});

app.MapControllers().RequireRateLimiting("fixed");

app.Run();
