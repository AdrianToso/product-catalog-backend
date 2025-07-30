using ADR_T.ProductCatalog.Core.Domain.Entities; 
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using MediatR; 

namespace ADR_T.ProductCatalog.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand> 
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken) 
    {
        var productToDelete = await _unitOfWork.ProductRepository.GetByIdAsync(request.Id, cancellationToken);

        if (productToDelete == null)
        {
            throw new NotFoundException(nameof(Product), request.Id);
        }

        await _unitOfWork.ProductRepository.DeleteAsync(productToDelete, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}