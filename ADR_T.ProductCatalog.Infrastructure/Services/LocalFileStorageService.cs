using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ADR_T.ProductCatalog.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly string _publicUrl;
    private readonly ILogger<LocalFileStorageService> _logger;
    private readonly IWebHostEnvironment _env;

    public LocalFileStorageService(IWebHostEnvironment env, IConfiguration config, ILogger<LocalFileStorageService> logger)
    {
        _logger = logger;
        _env = env;

        // Asegurar que la ruta base esté correctamente configurada
        _basePath = Path.Combine(env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "images", "products");
        _publicUrl = $"{config["App:BaseUrl"]?.TrimEnd('/')}/images/products";

        _logger.LogInformation("Inicializando LocalFileStorageService");
        _logger.LogInformation("Ruta base de almacenamiento: {BasePath}", _basePath);
        _logger.LogInformation("URL pública base: {PublicUrl}", _publicUrl);

        EnsureDirectoryExists();
    }

    private void EnsureDirectoryExists()
    {
        try
        {
            if (!Directory.Exists(_basePath))
            {
                _logger.LogInformation("Creando directorio: {BasePath}", _basePath);
                Directory.CreateDirectory(_basePath);
                _logger.LogInformation("Directorio creado exitosamente");
            }
            else
            {
                _logger.LogInformation("Directorio ya existe: {BasePath}", _basePath);
            }

            // Verificar permisos de escritura
            var testFile = Path.Combine(_basePath, $"test_{Guid.NewGuid()}.tmp");
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);
            _logger.LogInformation("Permisos de escritura verificados correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crítico al inicializar el directorio de almacenamiento");
            throw new InvalidOperationException($"No se pudo inicializar el almacenamiento de archivos: {ex.Message}", ex);
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        string uniqueName = null;

        try
        {
            _logger.LogInformation("Iniciando guardado de archivo: {FileName}", fileName);

            EnsureDirectoryExists();

            uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            var filePath = Path.Combine(_basePath, uniqueName);

            _logger.LogInformation("Guardando archivo como: {UniqueName}", uniqueName);
            _logger.LogInformation("Ruta completa: {FilePath}", filePath);

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous))
            {
                await fileStream.CopyToAsync(stream, cancellationToken);
            }

            var fileUrl = $"{_publicUrl}/{uniqueName}";
            _logger.LogInformation("Archivo guardado exitosamente en {ElapsedMs}ms: {FileUrl}",
                stopwatch.ElapsedMilliseconds, fileUrl);

            return fileUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar el archivo {FileName} como {UniqueName}", fileName, uniqueName);
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }
    }
}