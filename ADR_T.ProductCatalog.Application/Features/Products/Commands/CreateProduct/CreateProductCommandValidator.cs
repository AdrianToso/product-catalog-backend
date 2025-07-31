using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using FluentValidation; 

namespace ADR_T.ProductCatalog.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateProductCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no debe exceder los 100 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La descripción no debe exceder los 500 caracteres.");

        RuleFor(x => x.CategoryIds)
          .MustAsync(BeExistingCategories)
          .WithMessage("Una o más de las categorías especificadas no existen.");
    }
    private async Task<bool> BeExistingCategories(List<Guid> categoryIds, CancellationToken cancellationToken)
    {
        if (categoryIds == null || !categoryIds.Any())
        {
            return true;
        }

        var existingCategories = await _unitOfWork.CategoryRepository
            .ListAsync(c => categoryIds.Contains(c.Id), cancellationToken);

        return existingCategories.Count == categoryIds.Distinct().Count();
    }
}