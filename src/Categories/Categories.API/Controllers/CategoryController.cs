using Categories.Application.Categories.Commands.ChangeIcon;
using Categories.Application.Categories.Commands.CreateCategory;
using Categories.Application.Categories.Commands.CreateSystemCategory;
using Categories.Application.Categories.Commands.DeleteCategory;
using Categories.Application.Categories.Commands.RenameCategory;
using Categories.Application.Categories.Queries.GetCategoriesByProfileId;
using Categories.Application.Categories.Queries.GetCategoryById;
using Categories.Application.Categories.Queries.GetSystemCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Categories.API.Controllers;

[Authorize]
[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
  private readonly IMediator _mediator;

  public CategoryController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
  {
    var result = await _mediator.Send(command);
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpPost("system")]
  public async Task<IActionResult> CreateSystem([FromBody] CreateSystemCategoryCommand command)
  {
    var result = await _mediator.Send(command);
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    var result = await _mediator.Send(new GetCategoryByIdQuery(id));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpGet]
  public async Task<IActionResult> GetByProfileId([FromQuery] Guid profileId)
  {
    var result = await _mediator.Send(new GetCategoriesByProfileIdQuery(profileId));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpGet("system")]
  public async Task<IActionResult> GetSystem()
  {
    var result = await _mediator.Send(new GetSystemCategoriesQuery());
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpPut("{id}/rename")]
  public async Task<IActionResult> Rename(Guid id, [FromBody] string newName)
  {
    var result = await _mediator.Send(new RenameCategoryCommand(id, newName));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }

  [HttpPatch("{id}/icon")]
  public async Task<IActionResult> ChangeIcon(Guid id, [FromBody] string? newIcon)
  {
    var result = await _mediator.Send(new ChangeIconCategoryCommand(id, newIcon));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(Guid id)
  {
    var result = await _mediator.Send(new DeleteCategoryCommand(id));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }
}
