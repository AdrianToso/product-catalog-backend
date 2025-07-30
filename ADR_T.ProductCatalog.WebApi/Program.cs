using ADR_T.ProductCatalog.Application; 
using ADR_T.ProductCatalog.Infrastructure;
using ADR_T.ProductCatalog.WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Product Catalog API",
        Version = "v1",
        Description = "API para gestión de productos y categorías",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Adrián Toso",
            Url = new Uri("https://www.linkedin.com/in/adrian-toso-24b96419/")
        }
    });
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Catalog API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection(); 

app.UseAuthorization();

app.MapControllers();

app.Run(); 