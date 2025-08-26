using MediatR;
using Microsoft.AspNetCore.Http;

namespace ADR_T.ProductCatalog.Application.Features.Products.Commands.UpdateProductImage;

public record UpdateProductImageCommand(Guid ProductId, IFormFile File) : IRequest<string>;
