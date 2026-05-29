using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Correlation;

/// <summary>
/// Принимает X-Correlation-Id из входящего запроса (или генерирует новый),
/// кладёт его в HttpContext.Items и в логирующий scope, выставляет в response-заголовке.
/// </summary>
public class CorrelationIdMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<CorrelationIdMiddleware> _logger;

  public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    var correlationId = context.Request.Headers.TryGetValue(CorrelationIdContext.HeaderName, out var incoming) && !string.IsNullOrWhiteSpace(incoming)
      ? incoming.ToString()
      : Guid.NewGuid().ToString("N");

    context.Items[CorrelationIdContext.HttpContextItemKey] = correlationId;
    context.Response.Headers[CorrelationIdContext.HeaderName] = correlationId;

    using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
    {
      await _next(context);
    }
  }
}
