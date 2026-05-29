using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Users.Commands.ChangeEmail;
using Users.Application.Users.Commands.ChangePassword;
using Users.Application.Users.Commands.ChangeProfile;
using Users.Application.Users.Commands.DeleteUser;
using Users.Application.Users.Commands.LoginUser;
using Users.Application.Users.Commands.RegisterUser;
using Users.Application.Users.Queries.GetUserById;

namespace Users.API.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
  private readonly IMediator _mediator;

  public UserController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [AllowAnonymous]
  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
  {
    var result = await _mediator.Send(command);
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [AllowAnonymous]
  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
  {
    var result = await _mediator.Send(command);
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    var result = await _mediator.Send(new GetUserByIdQuery(id));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpPut("{id}/change-email")]
  public async Task<IActionResult> ChangeEmail(Guid id, [FromBody] string newEmail)
  {
    var result = await _mediator.Send(new ChangeEmailCommand(id, newEmail));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }

  [HttpPut("{id}/change-password")]
  public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordRequest request)
  {
    var result = await _mediator.Send(new ChangePasswordCommand(id, request.CurrentPassword, request.NewPassword));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }

  [HttpPut("{id}/change-profile")]
  public async Task<IActionResult> ChangeProfile(Guid id, [FromBody] ChangeProfileRequest request)
  {
    var result = await _mediator.Send(new ChangeProfileCommand(id, request.FirstName, request.LastName, request.PhoneNumber));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(Guid id)
  {
    var result = await _mediator.Send(new DeleteUserCommand(id));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }
}

public record ChangeProfileRequest(string? FirstName, string? LastName, string? PhoneNumber);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
