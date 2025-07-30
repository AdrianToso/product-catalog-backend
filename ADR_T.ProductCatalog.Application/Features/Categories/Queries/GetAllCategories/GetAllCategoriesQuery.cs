using ADR_T.ProductCatalog.Application.DTOs;
using MediatR;

namespace ADR_T.ProductCatalog.Application.Features.Categories.Queries.GetAllCategories;
public record GetAllCategoriesQuery : IRequest<List<CategoryDto>>;
