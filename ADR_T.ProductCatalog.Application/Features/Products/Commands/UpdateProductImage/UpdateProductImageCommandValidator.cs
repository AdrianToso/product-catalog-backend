using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace ADR_T.ProductCatalog.Application.Features.Products.Commands.UpdateProductImage
{
    public class UpdateProductImageCommandValidator : AbstractValidator<UpdateProductImageCommand>
    {
        public UpdateProductImageCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("El ID del producto es obligatorio.");

            RuleFor(x => x.File)
                .NotNull().WithMessage("El archivo es obligatorio.")
                .Must(f => f != null && f.Length > 0).WithMessage("El archivo no puede estar vacío.")
                .Must(f => f != null && IsImage(f)).WithMessage("Solo se permiten archivos de imagen.");
        }

        private bool IsImage(IFormFile file)
        {
            if (file == null) return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };

            var extension = Path.GetExtension(file.FileName)?.ToLower();
            var contentType = file.ContentType?.ToLower();

            return allowedExtensions.Contains(extension) && allowedContentTypes.Contains(contentType);
        }
    }
}