using MediatR;
using Microsoft.AspNetCore.Http;
namespace ADR_T.ProductCatalog.Application.Features.Products.Commands.CreateProduct;
public record CreateProductWithImageCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public IFormFile ImageFile { get; set; } = null!;
}