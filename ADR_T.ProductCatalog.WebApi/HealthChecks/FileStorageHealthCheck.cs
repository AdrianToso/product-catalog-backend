using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ADR_T.ProductCatalog.WebApi.HealthChecks
{
    public class FileStorageHealthCheck : IHealthCheck
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FileStorageHealthCheck> _logger;
        private readonly string _storagePath;

        public FileStorageHealthCheck(IWebHostEnvironment env, ILogger<FileStorageHealthCheck> logger)
        {
            _env = env;
            _logger = logger;
            // Define la ruta a verificar, la misma que usa LocalFileStorageService
            _storagePath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "images", "products");
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Asegurarse de que el directorio existe
                if (!Directory.Exists(_storagePath))
                {
                    Directory.CreateDirectory(_storagePath);
                }

                // Intentar crear un archivo temporal para verificar permisos de escritura
                var testFilePath = Path.Combine(_storagePath, $"healthcheck_{Guid.NewGuid()}.tmp");
                File.WriteAllText(testFilePath, "Health Check Test");
                File.Delete(testFilePath);

                _logger.LogInformation("Verificación de estado del almacenamiento de archivos exitosa.");
                return Task.FromResult(HealthCheckResult.Healthy("El directorio de almacenamiento está accesible y tiene permisos de escritura."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Verificación de estado del almacenamiento de archivos fallida.");
                return Task.FromResult(HealthCheckResult.Unhealthy("Error al acceder al directorio de almacenamiento.", ex));
            }
        }
    }
}
