using ADR_T.ProductCatalog.Application.DTOs;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace ADR_T.ProductCatalog.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _unitOfWork.ProductRepository.GetAllWithCategoriesAsync(cancellationToken);

        return _mapper.Map<List<ProductDto>>(products);
    }
}