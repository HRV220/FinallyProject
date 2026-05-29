using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transactions.Application.Wallets.Commands.ChangeCurrency;
using Transactions.Application.Wallets.Commands.ChangeIcon;
using Transactions.Application.Wallets.Commands.ChangeNote;
using Transactions.Application.Wallets.Commands.ChangeSortOrder;
using Transactions.Application.Wallets.Commands.CreateWallet;
using Transactions.Application.Wallets.Commands.DeleteWallet;
using Transactions.Application.Wallets.Commands.RenameWallet;
using Transactions.Application.Wallets.Queries.GetWalletById;
using Transactions.Application.Wallets.Queries.GetWalletsByProfileId;

namespace Transactions.API.Controllers;

[Authorize]
[ApiController]
[Route("api/wallets")]
public class WalletController : ControllerBase
{
  private readonly IMediator _mediator;

  public WalletController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateWalletCommand command)
  {
    var result = await _mediator.Send(command);
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    var result = await _mediator.Send(new GetWalletByIdQuery(id));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpGet]
  public async Task<IActionResult> GetByProfileId([FromQuery] Guid profileId)
  {
    var result = await _mediator.Send(new GetWalletsByProfileIdQuery(profileId));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpPut("{id}/rename")]
  public async Task<IActionResult> Rename(Guid id, [FromBody] string newName)
  {
    var result = await _mediator.Send(new RenameWalletCommand(id, newName));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }

  [HttpPatch("{id}/sort-order")]
  public async Task<IActionResult> ChangeSortOrder(Guid id, [FromBody] int newSortOrder)
  {
    var result = await _mediator.Send(new ChangeSortOrderCommand(id, newSortOrder));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }

  [HttpPatch("{id}/icon")]
  public async Task<IActionResult> ChangeIcon(Guid id, [FromBody] string? newIcon)
  {
    var result = await _mediator.Send(new ChangeIconCommand(id, newIcon));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }

  [HttpPatch("{id}/currency")]
  public async Task<IActionResult> ChangeCurrency(Guid id, [FromBody] Guid newCurrencyId)
  {
    var result = await _mediator.Send(new ChangeCurrencyCommand(id, newCurrencyId));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }

  [HttpPatch("{id}/note")]
  public async Task<IActionResult> ChangeNote(Guid id, [FromBody] string? newNote)
  {
    var result = await _mediator.Send(new ChangeNoteCommand(id, newNote));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(Guid id)
  {
    var result = await _mediator.Send(new DeleteWalletCommand(id));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }
}
