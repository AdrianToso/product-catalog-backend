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
        const int maxPageSize = 50;
        var pageSize = request.PageSize > maxPageSize ? maxPageSize : request.PageSize;
        var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;

        var totalCount = await _unitOfWork.ProductRepository.CountAsync(cancellationToken);
        var products = await _unitOfWork.ProductRepository.GetAllWithCategoriesPagedAsync(pageNumber, pageSize, cancellationToken);
        var productDtos = _mapper.Map<List<ProductDto>>(products);

        return new PagedResponse<ProductDto>(productDtos, pageNumber, pageSize, totalCount);
    }
}