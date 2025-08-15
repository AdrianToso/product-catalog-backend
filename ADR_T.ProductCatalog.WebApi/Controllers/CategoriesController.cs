using ADR_T.ProductCatalog.Application.Features.Categories.Commands.CreateCategory;
using ADR_T.ProductCatalog.Application.Features.Categories.Commands.DeleteCategory;
using ADR_T.ProductCatalog.Application.Features.Categories.Commands.UpdateCategory;
using ADR_T.ProductCatalog.Application.Features.Categories.Queries.GetAllCategories;
using ADR_T.ProductCatalog.Application.Features.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ADR_T.ProductCatalog.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea una nueva categoría.
    /// </summary>
    /// <param name="command">Datos para la nueva categoría.</param>
    /// <returns>La ubicación de la nueva categoría creada.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Editor")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCategoryById), new { id }, new { id });
    }

    /// <summary>
    /// Obtiene una categoría por su ID.
    /// </summary>
    /// <param name="id">El ID de la categoría.</param>
    /// <returns>La categoría solicitada.</returns>
    [HttpGet("{id:guid}")]
    [AllowAnonymous] // Este endpoint puede ser público
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Obtiene todas las categorías.
    /// </summary>
    /// <returns>Una lista de todas las categorías.</returns>
    [HttpGet]
    [AllowAnonymous] // Este endpoint también puede ser público
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCategories()
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery());
        return Ok(result);
    }

    /// <summary>
    /// Actualiza una categoría existente.
    /// </summary>
    /// <param name="id">El ID de la categoría a actualizar.</param>
    /// <param name="command">Los nuevos datos de la categoría.</param>
    /// <returns>Una respuesta indicando que la operación fue exitosa.</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Editor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryCommand command)
    {
        if (id != command.Id)
            return BadRequest("El ID en la URL no coincide con el del cuerpo.");

        await _mediator.Send(command);
        return Ok(new { message = "Categoría actualizada correctamente." });
    }

    /// <summary>
    /// Elimina una categoría por su ID.
    /// </summary>
    /// <param name="id">El ID de la categoría a eliminar.</param>
    /// <returns>Una respuesta sin contenido.</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        await _mediator.Send(new DeleteCategoryCommand(id));
        return NoContent();
    }
}