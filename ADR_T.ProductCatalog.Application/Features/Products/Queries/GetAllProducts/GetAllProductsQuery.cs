using ADR_T.ProductCatalog.Application.DTOs;
using ADR_T.ProductCatalog.Application.DTOs.Common;
using MediatR;

namespace ADR_T.ProductCatalog.Application.Features.Products.Queries.GetAllProducts;

public record GetAllProductsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResponse<ProductDto>>;
