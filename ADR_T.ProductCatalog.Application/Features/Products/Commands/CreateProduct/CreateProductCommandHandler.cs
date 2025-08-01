using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Exceptions; // Necesario para ValidationException
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
        var product = new Product(request.Name, request.Description, request.CategoryId, request.ImageUrl);

        await _unitOfWork.ProductRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return product.Id; 
    }
}