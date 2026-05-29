using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transactions.Application.Transactions.Commands.ChangeCategory;
using Transactions.Application.Transactions.Commands.ChangeDescription;
using Transactions.Application.Transactions.Commands.CreateTransaction;
using Transactions.Application.Transactions.Commands.DeleteTransaction;
using Transactions.Application.Transactions.Queries.GetTransactionById;
using Transactions.Application.Transactions.Queries.GetTransactionsByWalletId;
using Transactions.Application.Transactions.Queries.GetWalletBalance;

namespace Transactions.API.Controllers;

[Authorize]
[ApiController]
[Route("api/transactions")]
public class TransactionController : ControllerBase
{
  private readonly IMediator _mediator;

  public TransactionController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreateTransactionCommand command)
  {
    var result = await _mediator.Send(command);
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    var result = await _mediator.Send(new GetTransactionByIdQuery(id));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpGet]
  public async Task<IActionResult> GetByWalletId([FromQuery] Guid walletId)
  {
    var result = await _mediator.Send(new GetTransactionsByWalletIdQuery(walletId));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpGet("balance")]
  public async Task<IActionResult> GetWalletBalance([FromQuery] Guid walletId)
  {
    var result = await _mediator.Send(new GetWalletBalanceQuery(walletId));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpPatch("{id}/description")]
  public async Task<IActionResult> ChangeDescription(Guid id, [FromBody] string? newDescription)
  {
    var result = await _mediator.Send(new ChangeDescriptionCommand(id, newDescription));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }

  [HttpPatch("{id}/category")]
  public async Task<IActionResult> ChangeCategory(Guid id, [FromBody] Guid? newCategoryId)
  {
    var result = await _mediator.Send(new ChangeCategoryTransactionCommand(id, newCategoryId));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(Guid id)
  {
    var result = await _mediator.Send(new DeleteTransactionCommand(id));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }
}
