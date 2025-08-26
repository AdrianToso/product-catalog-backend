using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace ADR_T.ProductCatalog.Application.Common.Helpers
{
    public static class ImageValidationHelper
    {
        private static readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        private static readonly string[] _allowedMimeTypes = { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };
        private const long _maxFileSize = 5 * 1024 * 1024; // 5MB

        public static bool IsValidImage(IFormFile file, [NotNullWhen(false)] out string? errorMessage)
        {
            errorMessage = null;

            // Validar que el archivo no sea nulo
            if (file == null)
            {
                errorMessage = "El archivo es requerido.";
                return false;
            }

            // Validar tamaño
            if (file.Length == 0)
            {
                errorMessage = "El archivo está vacío.";
                return false;
            }

            if (file.Length > _maxFileSize)
            {
                errorMessage = $"El archivo excede el tamaño máximo de {_maxFileSize / 1024 / 1024}MB.";
                return false;
            }

            // Validar extensión
            var extension = Path.GetExtension(file.FileName)?.ToLower();
            if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
            {
                errorMessage = $"Extensión no permitida. Extensiones válidas: {string.Join(", ", _allowedExtensions)}";
                return false;
            }

            // Validar tipo MIME
            if (string.IsNullOrEmpty(file.ContentType) || !_allowedMimeTypes.Contains(file.ContentType.ToLower()))
            {
                errorMessage = $"Tipo de archivo no permitido. Tipos válidos: {string.Join(", ", _allowedMimeTypes)}";
                return false;
            }

            return true;
        }

        // Método sobrecargado para uso simple (sin mensaje de error)
        public static bool IsValidImage(IFormFile file)
        {
            return IsValidImage(file, out _);
        }
    }
}