using ADR_T.ProductCatalog.Core.Domain.Entities; 
using ADR_T.ProductCatalog.Core.Domain.Exceptions; 
using ADR_T.ProductCatalog.Core.Domain.Interfaces; 
using MediatR; 

namespace ADR_T.ProductCatalog.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand> 
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var productToUpdate = await _unitOfWork.ProductRepository.GetByIdWithCategoriesAsync(request.Id, cancellationToken);

        if (productToUpdate == null)
        {
            throw new NotFoundException(nameof(Product), request.Id);
        }

        productToUpdate.Update(request.Name, request.Description, request.ImageUrl);

        productToUpdate.Categories.Clear();

        if (request.CategoryIds != null && request.CategoryIds.Any())
        {
            var categories = await _unitOfWork.CategoryRepository.ListAsync(c => request.CategoryIds.Contains(c.Id), cancellationToken);

            if (categories.Count() != request.CategoryIds.Count())
            {
                var missingCategoryIds = request.CategoryIds.Except(categories.Select(c => c.Id)).ToList();
                if (missingCategoryIds.Any())
                {
                    var errors = new Dictionary<string, string[]>();
                    errors.Add(nameof(request.CategoryIds), new[] { $"Las categorías con IDs: {string.Join(", ", missingCategoryIds)} no fueron encontradas." });
                    throw new ValidationException(errors);
                }
            }

            foreach (var category in categories)
            {
                productToUpdate.Categories.Add(category);
            }
        }

        await _unitOfWork.ProductRepository.UpdateAsync(productToUpdate, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}