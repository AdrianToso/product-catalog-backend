using ADR_T.ProductCatalog.Application.Features.Products.Commands.CreateProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ADR_T.ProductCatalog.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var productId = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetProductById), new { id = productId }, new { id = productId });
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetProductById(Guid id)
    {
        return Ok($"Endpoint para obtener el producto {id} pendiente de implementación.");
    }
}