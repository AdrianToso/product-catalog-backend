using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using FluentValidation;

namespace ADR_T.ProductCatalog.Application.Features.Products.Commands;
public abstract class ProductCommandValidatorBase<T> : AbstractValidator<T>
{
    private readonly IUnitOfWork _unitOfWork;

    protected ProductCommandValidatorBase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    protected async Task<bool> BeExistingCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        if (categoryId == Guid.Empty)
            return false;

        return await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId, cancellationToken) != null;
    }
}