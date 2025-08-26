using ADR_T.ProductCatalog.Application.Features.Products.Commands.CreateProduct;
using ADR_T.ProductCatalog.Core.Domain.Entities;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using MediatR;

namespace ADR_T.ProductCatalog.Application.Features.Products.Commands.CreateProduct;

public class CreateProductWithImageCommandHandler : IRequestHandler<CreateProductWithImageCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public CreateProductWithImageCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<Guid> Handle(CreateProductWithImageCommand request, CancellationToken cancellationToken)
    {
        string imageUrl = null;

        // Subir imagen solo si se proporciona
        if (request.ImageFile != null && request.ImageFile.Length > 0)
        {
            using var stream = request.ImageFile.OpenReadStream();
            imageUrl = await _fileStorageService.SaveFileAsync(
                stream,
                request.ImageFile.FileName,
                cancellationToken);
        }

        // Crear producto
        var product = new Product(request.Name, request.Description, request.CategoryId, imageUrl);
        await _unitOfWork.ProductRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return product.Id;
    }
}