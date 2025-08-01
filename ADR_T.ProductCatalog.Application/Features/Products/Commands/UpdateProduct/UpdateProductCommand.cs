using MediatR;

namespace ADR_T.ProductCatalog.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand : IRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public Guid CategoryId { get; set; }
}