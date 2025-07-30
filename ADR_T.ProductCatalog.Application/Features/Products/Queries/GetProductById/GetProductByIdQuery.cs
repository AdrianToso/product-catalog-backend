using ADR_T.ProductCatalog.Application.DTOs;
using MediatR;

namespace ADR_T.ProductCatalog.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;
