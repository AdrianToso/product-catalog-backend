using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using MediatR;

namespace ADR_T.ProductCatalog.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
 
        var newProduct = new Product(request.Name, request.Description, request.ImageUrl);

        if (request.CategoryIds != null && request.CategoryIds.Any())
        {
            var categories = await _unitOfWork.CategoryRepository.ListAsync(c => request.CategoryIds.Contains(c.Id), cancellationToken);
            foreach (var category in categories)
            {
                newProduct.Categories.Add(category);
            }
        }

        await _unitOfWork.ProductRepository.AddAsync(newProduct, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return newProduct.Id;
    }
}