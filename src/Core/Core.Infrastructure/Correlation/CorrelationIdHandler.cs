using Microsoft.AspNetCore.Http;

namespace Core.Infrastructure.Correlation;

public class CorrelationIdHandler : DelegatingHandler
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public CorrelationIdHandler(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    if (!request.Headers.Contains(CorrelationIdContext.HeaderName))
    {
      var ctx = _httpContextAccessor.HttpContext;
      string? correlationId = ctx?.Items[CorrelationIdContext.HttpContextItemKey] as string;
      if (string.IsNullOrEmpty(correlationId))
        correlationId = Guid.NewGuid().ToString("N");
      request.Headers.Add(CorrelationIdContext.HeaderName, correlationId);
    }

    return base.SendAsync(request, cancellationToken);
  }
}
