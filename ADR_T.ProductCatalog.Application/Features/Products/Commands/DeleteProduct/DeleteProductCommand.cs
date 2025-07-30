using MediatR;

namespace ADR_T.ProductCatalog.Application.Features.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest;