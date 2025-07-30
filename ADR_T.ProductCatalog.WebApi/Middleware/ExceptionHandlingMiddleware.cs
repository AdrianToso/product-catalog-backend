using System.Net; 
using System.Text.Json;
using ADR_T.ProductCatalog.Core.Domain.Exceptions; 
using Microsoft.AspNetCore.Mvc;

namespace ADR_T.ProductCatalog.WebAPI.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
          
            _logger.LogError(ex, "An unhandled exception occurred during request processing.");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json"; 

        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "Uno o mas errores en el request.",
            Detail = "An unexpected internal server error has occurred. Please try again later.",
            Instance = context.Request.Path 
        };

        switch (exception)
        {
            case ValidationException validationException:
                problemDetails.Status = (int)HttpStatusCode.BadRequest; 
                problemDetails.Title = "Error de validacion";
                problemDetails.Detail = "Uno o mas errores en el request.";
                problemDetails.Extensions["errors"] = validationException.Errors;
                break;
            case NotFoundException notFoundException:
                problemDetails.Status = (int)HttpStatusCode.NotFound; 
                problemDetails.Title = "Not Found";
                problemDetails.Detail = notFoundException.Message;
                break;
           
            default:
                if (context.RequestServices.GetService<IHostEnvironment>()?.IsDevelopment() == true)
                {
                    problemDetails.Detail = exception.Message;
                }
                break;
        }

        context.Response.StatusCode = problemDetails.Status.Value;
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}