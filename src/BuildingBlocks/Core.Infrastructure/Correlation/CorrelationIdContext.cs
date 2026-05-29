namespace Core.Infrastructure.Correlation;

public static class CorrelationIdContext
{
  public const string HeaderName = "X-Correlation-Id";
  public const string HttpContextItemKey = "__correlation_id";
}
