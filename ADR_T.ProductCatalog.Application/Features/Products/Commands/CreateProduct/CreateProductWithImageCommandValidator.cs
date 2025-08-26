using ADR_T.ProductCatalog.Application.Common.Helpers;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using FluentValidation;

namespace ADR_T.ProductCatalog.Application.Features.Products.Commands.CreateProduct;

public class CreateProductWithImageCommandValidator : AbstractValidator<CreateProductWithImageCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductWithImageCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no debe exceder los 100 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La descripción no debe exceder los 500 caracteres.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("El ID de la categoría es requerido.")
            .MustAsync(BeExistingCategory).WithMessage("La categoría especificada no existe.");

        // Hacer la imagen opcional
        RuleFor(x => x.ImageFile)
            .Custom((file, context) =>
            {
                if (file != null && !ImageValidationHelper.IsValidImage(file, out var errorMessage))
                {
                    context.AddFailure(errorMessage);
                }
            });
    }

    private async Task<bool> BeExistingCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId, cancellationToken) != null;
    }
}