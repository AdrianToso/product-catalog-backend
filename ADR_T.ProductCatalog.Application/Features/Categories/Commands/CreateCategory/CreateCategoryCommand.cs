using MediatR;

namespace ADR_T.ProductCatalog.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string? Description) : IRequest<Guid>;