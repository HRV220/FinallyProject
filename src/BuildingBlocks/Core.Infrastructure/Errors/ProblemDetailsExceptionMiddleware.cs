using System.Text.Json;
using Core.Infrastructure.Correlation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Errors;

/// <summary>
/// Любые необработанные исключения превращает в RFC 7807 ProblemDetails,
/// дополняя их CorrelationId. Полные стек-трейсы возвращаются только в Development.
/// </summary>
public class ProblemDetailsExceptionMiddleware
{
  private static readonly JsonSerializerOptions JsonOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  private readonly RequestDelegate _next;
  private readonly IHostEnvironment _env;
  private readonly ILogger<ProblemDetailsExceptionMiddleware> _logger;

  public ProblemDetailsExceptionMiddleware(RequestDelegate next, IHostEnvironment env, ILogger<ProblemDetailsExceptionMiddleware> logger)
  {
    _next = next;
    _env = env;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unhandled exception while processing request {Method} {Path}.", context.Request.Method, context.Request.Path);
      await WriteProblemAsync(context, ex);
    }
  }

  private async Task WriteProblemAsync(HttpContext context, Exception ex)
  {
    if (context.Response.HasStarted)
      return;

    var correlationId = context.Items[CorrelationIdContext.HttpContextItemKey] as string ?? string.Empty;

    var problem = new ProblemDetails
    {
      Type = "https://datatracker.ietf.org/doc/html/rfc7807",
      Title = "An unexpected error occurred.",
      Status = StatusCodes.Status500InternalServerError,
      Detail = _env.IsDevelopment() ? ex.ToString() : ex.Message,
      Instance = context.Request.Path
    };
    problem.Extensions["correlationId"] = correlationId;

    context.Response.StatusCode = problem.Status.Value;
    context.Response.ContentType = "application/problem+json";
    await context.Response.WriteAsync(JsonSerializer.Serialize(problem, JsonOptions));
  }
}
