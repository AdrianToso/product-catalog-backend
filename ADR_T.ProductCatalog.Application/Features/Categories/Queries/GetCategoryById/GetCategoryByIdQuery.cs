using ADR_T.ProductCatalog.Application.DTOs;
using MediatR;

namespace ADR_T.ProductCatalog.Application.Features.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;
