using ADR_T.ProductCatalog.Application.DTOs;
using ADR_T.ProductCatalog.Application.DTOs.Common;
using ADR_T.ProductCatalog.Application.Features.Products.Commands.CreateProduct;
using ADR_T.ProductCatalog.Application.Features.Products.Commands.DeleteProduct;
using ADR_T.ProductCatalog.Application.Features.Products.Commands.UpdateProduct;
using ADR_T.ProductCatalog.Application.Features.Products.Queries.GetAllProducts;
using ADR_T.ProductCatalog.Application.Features.Products.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ADR_T.ProductCatalog.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene una lista paginada de productos.
    /// </summary>
    /// <param name="pageNumber">Número de página (por defecto 1).</param>
    /// <param name="pageSize">Tamaño de página (por defecto 10).</param>
    /// <returns>Una respuesta paginada con DTOs de producto.</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResponse<ProductDto>), 200)]
    public async Task<ActionResult<PagedResponse<ProductDto>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetAllProductsQuery { PageNumber = pageNumber, PageSize = pageSize };
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Obtiene un producto por su ID.
    /// </summary>
    /// <param name="id">ID del producto.</param>
    /// <returns>El DTO del producto.</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
       
        var query = new GetProductByIdQuery(id);
        var product = await _mediator.Send(query);
        return Ok(product);
    }

    /// <summary>
    /// Crea un nuevo producto.
    /// </summary>
    /// <param name="command">Comando con los datos del nuevo producto.</param>
    /// <returns>El ID del producto creado.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateProductCommand command)
    {
        var productId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = productId }, productId);
    }

    /// <summary>
    /// Actualiza un producto existente.
    /// </summary>
    /// <param name="id">ID del producto a actualizar.</param>
    /// <param name="command">Comando con los datos actualizados del producto.</param>
    /// <returns>No Content si la actualización fue exitosa.</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Editor")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Elimina un producto.
    /// </summary>
    /// <param name="id">ID del producto a eliminar.</param>
    /// <returns>No Content si la eliminación fue exitosa.</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Delete(Guid id)
    {
        var command = new DeleteProductCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}