using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Profiles.Commands.CreateProfile;
using Users.Application.Profiles.Commands.DeleteProfile;
using Users.Application.Profiles.Commands.RenameProfile;
using Users.Application.Profiles.Commands.ToggleProfileActive;
using Users.Application.Profiles.Queries.GetProfileById;
using Users.Application.Profiles.Queries.GetProfilesByUserId;

namespace Users.API.Controllers;

[Authorize]
[ApiController]
[Route("api/profiles")]
public class ProfileController : ControllerBase
{
  private readonly IMediator _mediator;

  public ProfileController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateProfileCommand command)
  {
    var result = await _mediator.Send(command);
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpGet]
  public async Task<IActionResult> GetByUserId([FromQuery] GetProfilesByUserIdQuery query)
  {
    var result = await _mediator.Send(query);
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    var result = await _mediator.Send(new GetProfileByIdQuery(id));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpPut("{id}/rename")]
  public async Task<IActionResult> Rename(Guid id, [FromBody] string newName)
  {
    var result = await _mediator.Send(new RenameProfileCommand(id, newName));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }

  [HttpPatch("{id}/toggle-active")]
  public async Task<IActionResult> ToggleActive(Guid id)
  {
    var result = await _mediator.Send(new ToggleProfileActiveCommand(id));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(Guid id)
  {
    var result = await _mediator.Send(new DeleteProfileCommand(id));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }
}
