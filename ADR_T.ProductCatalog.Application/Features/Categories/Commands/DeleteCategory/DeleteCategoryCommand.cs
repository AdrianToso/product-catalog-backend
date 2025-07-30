using MediatR;

namespace ADR_T.ProductCatalog.Application.Features.Categories.Commands.DeleteCategory;
public record DeleteCategoryCommand(Guid Id) : IRequest;

