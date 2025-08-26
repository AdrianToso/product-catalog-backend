using ADR_T.ProductCatalog.Application.DTOs;
using ADR_T.ProductCatalog.Application.DTOs.Common;
using ADR_T.ProductCatalog.Application.Features.Products.Commands.CreateProduct;
using ADR_T.ProductCatalog.Application.Features.Products.Commands.DeleteProduct;
using ADR_T.ProductCatalog.Application.Features.Products.Commands.UpdateProduct;
using ADR_T.ProductCatalog.Application.Features.Products.Commands.UpdateProductImage;
using ADR_T.ProductCatalog.Application.Features.Products.Queries.GetAllProducts;
using ADR_T.ProductCatalog.Application.Features.Products.Queries.GetProductById;
using ADR_T.ProductCatalog.Core.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ADR_T.ProductCatalog.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ProductsController>();
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

    [HttpPost("{id:guid}/image")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest("El archivo es obligatorio y no puede estar vacío.");

            // Validación adicional de tipo de archivo
            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };
            if (!allowedContentTypes.Contains(file.ContentType.ToLower()))
                return BadRequest("Solo se permiten archivos de imagen (JPEG, PNG, GIF, BMP, WEBP).");

            var command = new UpdateProductImageCommand(id, file);
            var imageUrl = await _mediator.Send(command, cancellationToken);

            return Ok(new { ImageUrl = imageUrl });
        }
        catch (ValidationException ex)
        {
            // Log detallado de errores de validación
            foreach (var error in ex.Errors)
            {
                _logger.LogWarning("Error de validación: {PropertyName} - {ErrorMessages}", error.Key, string.Join(", ", error.Value));
            }
            return BadRequest(ex.Errors);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Producto no encontrado: {ProductId}", id);
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar la imagen para el producto {ProductId}", id);
            return StatusCode(500, "Error interno al procesar la imagen");
        }
    }

    [HttpPost("debug-upload")]
    [AllowAnonymous]
    public async Task<IActionResult> DebugUpload(IFormFile file)
    {
        try
        {
            if (file == null)
                return BadRequest("File is null");

            return Ok(new
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Length = file.Length,
                Headers = file.Headers,
                CanRead = file.OpenReadStream().CanRead
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message, Details = ex.ToString() });
        }
    }
    [HttpPost("with-image")]
    [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateWithImage([FromForm] CreateProductWithImageCommand command)
    {
        var productId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = productId }, productId);
    }
}