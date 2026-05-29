using System.Security.Claims;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reports.Application.Abstractions;
using Reports.Application.Reports.Commands.CreateReport;
using Reports.Application.Reports.Queries.GetReportDownloadUrl;
using Reports.Application.Reports.Queries.GetReportStatus;
using Reports.Domain.Enums;

namespace Reports.API.Controllers;

[Authorize]
[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
  private static readonly JsonSerializerOptions JsonOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  private readonly IMediator _mediator;
  private readonly IFileStorage _fileStorage;

  public ReportsController(IMediator mediator, IFileStorage fileStorage)
  {
    _mediator = mediator;
    _fileStorage = fileStorage;
  }

  [HttpPost("profile-transactions")]
  public async Task<IActionResult> CreateProfileTransactionsReport(
    [FromBody] CreateProfileTransactionsReportRequest request,
    CancellationToken ct)
  {
    var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
    if (!Guid.TryParse(userIdString, out var userId))
      return Unauthorized();

    if (request.From > request.To)
      return BadRequest(new { error = "'From' date must be before or equal to 'To' date." });

    var parameters = JsonSerializer.Serialize(new
    {
      profileId = request.ProfileId,
      from = request.From,
      to = request.To
    }, JsonOptions);

    var result = await _mediator.Send(new CreateReportCommand(ReportType.ProfileTransactions, userId, parameters), ct);
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Accepted(result.Value);
  }

  [HttpPost("category-breakdown")]
  public async Task<IActionResult> CreateCategoryBreakdownReport(
    [FromBody] CreateCategoryBreakdownReportRequest request,
    CancellationToken ct)
  {
    var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
    if (!Guid.TryParse(userIdString, out var userId))
      return Unauthorized();

    if (request.From > request.To)
      return BadRequest(new { error = "'From' date must be before or equal to 'To' date." });

    var parameters = JsonSerializer.Serialize(new
    {
      profileId = request.ProfileId,
      from = request.From,
      to = request.To
    }, JsonOptions);

    var result = await _mediator.Send(new CreateReportCommand(ReportType.CategoryBreakdown, userId, parameters), ct);
    if (result.IsFailure)
      return BadRequest(result.Error);
    return Accepted(result.Value);
  }

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetStatus(Guid id, CancellationToken ct)
  {
    var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
    if (!Guid.TryParse(userIdString, out var userId))
      return Unauthorized();

    var result = await _mediator.Send(new GetReportStatusQuery(id, userId), ct);
    if (result.IsFailure)
    {
      if (result.Error!.Code == "Report.NotFound")
        return NotFound(result.Error);
      return BadRequest(result.Error);
    }
    return Ok(result.Value);
  }

  [HttpGet("{id:guid}/download")]
  public async Task<IActionResult> GetDownloadUrl(Guid id, CancellationToken ct)
  {
    var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
    if (!Guid.TryParse(userIdString, out var userId))
      return Unauthorized();

    var result = await _mediator.Send(new GetReportDownloadUrlQuery(id, userId), ct);
    if (result.IsFailure)
    {
      if (result.Error!.Code == "Report.NotFound")
        return NotFound(result.Error);
      if (result.Error.Code == "Report.NotReady")
        return Conflict(result.Error);
      return BadRequest(result.Error);
    }
    return Ok(result.Value);
  }

  [HttpGet("files/{*key}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetFile(string key, CancellationToken ct)
  {
    if (string.IsNullOrWhiteSpace(key))
      return NotFound();

    var stream = await _fileStorage.OpenReadAsync(key, ct);
    if (stream is null)
      return NotFound();

    return File(stream, "text/csv; charset=utf-8", Path.GetFileName(key));
  }
}
