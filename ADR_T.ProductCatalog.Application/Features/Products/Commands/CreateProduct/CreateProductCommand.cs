using MediatR;

namespace ADR_T.ProductCatalog.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    string? ImageUrl,
    Guid CategoryId) : IRequest<Guid>;
