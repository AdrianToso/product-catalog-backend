using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ADR_T.ProductCatalog.WebApi.Middleware
{
    /// <summary>
    /// Middleware para registrar las solicitudes HTTP y medir el tiempo total de ejecución.
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation(
                "Request iniciado: {Method} {Path} desde {IpAddress} | CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                context.Connection.RemoteIpAddress?.ToString(),
                context.TraceIdentifier
            );

            await _next(context);

            stopwatch.Stop();

            _logger.LogInformation(
                "Request finalizado: {Method} {Path} - Estado: {StatusCode} - Tiempo: {Elapsed} ms | CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                context.TraceIdentifier
            );
        }
    }
}
