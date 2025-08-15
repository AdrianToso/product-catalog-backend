using ADR_T.ProductCatalog.Application.DTOs;
using ADR_T.ProductCatalog.Application.DTOs.Common;
using ADR_T.ProductCatalog.Core.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace ADR_T.ProductCatalog.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PagedResponse<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResponse<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var totalCount = await _unitOfWork.ProductRepository.CountAsync(cancellationToken);
        var products = await _unitOfWork.ProductRepository.GetAllWithCategoriesPagedAsync(request.PageNumber, request.PageSize, cancellationToken);
        var productDtos = _mapper.Map<List<ProductDto>>(products);

        return new PagedResponse<ProductDto>(productDtos, request.PageNumber, request.PageSize, totalCount);
    }
}