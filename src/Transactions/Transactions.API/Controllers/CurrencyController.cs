using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transactions.Application.Currencies.Commands.UpdateCurrencyRate;
using Transactions.Application.Currencies.Queries.GetAllCurrencies;
using Transactions.Application.Currencies.Queries.GetCurrencyById;

namespace Transactions.API.Controllers;

[Authorize]
[ApiController]
[Route("api/currencies")]
public class CurrencyController : ControllerBase
{
  private readonly IMediator _mediator;

  public CurrencyController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    var result = await _mediator.Send(new GetAllCurrenciesQuery());
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    var result = await _mediator.Send(new GetCurrencyByIdQuery(id));
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Ok(result.Value);
  }

  [HttpPatch("{id}/rate")]
  public async Task<IActionResult> UpdateRate(Guid id, [FromBody] UpdateCurrencyRateCommand command)
  {
    var result = await _mediator.Send(command with { Id = id });
    if (result.IsFailure)
      return BadRequest(result.Error);
    return NoContent();
  }
}
