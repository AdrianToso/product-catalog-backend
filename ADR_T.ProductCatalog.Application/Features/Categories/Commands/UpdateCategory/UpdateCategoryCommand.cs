using MediatR;

namespace ADR_T.ProductCatalog.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(Guid Id, string Name, string? Description) : IRequest;
