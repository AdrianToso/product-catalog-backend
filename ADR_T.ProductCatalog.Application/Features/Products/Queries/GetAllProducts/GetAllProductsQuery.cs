using ADR_T.ProductCatalog.Application.DTOs;
using MediatR;

namespace ADR_T.ProductCatalog.Application.Features.Products.Queries.GetAllProducts;

public record GetAllProductsQuery() : IRequest<List<ProductDto>>;
