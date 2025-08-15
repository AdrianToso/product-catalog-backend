using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ADR_T.ProductCatalog.Application.Common.Behaviors
{
    /// <summary>
    /// Pipeline Behavior que mide el tiempo de ejecución de cada comando/consulta y registra errores.
    /// </summary>
    public class PerformanceLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<PerformanceLoggingBehavior<TRequest, TResponse>> _logger;

        public PerformanceLoggingBehavior(ILogger<PerformanceLoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("▶️ Inicio de {RequestName} con datos: {@Request}", typeof(TRequest).Name, request);

                var response = await next();

                stopwatch.Stop();
                _logger.LogInformation("⏱ Fin de {RequestName} - Tiempo: {Elapsed} ms", typeof(TRequest).Name, stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "❌ Error en {RequestName} después de {Elapsed} ms", typeof(TRequest).Name, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
