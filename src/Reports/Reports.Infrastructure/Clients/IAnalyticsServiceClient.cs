using Refit;

namespace Reports.Infrastructure.Clients;

public interface IAnalyticsServiceClient
{
  [Get("/api/analytics/categories")]
  Task<IReadOnlyList<CategoryAnalyticsDto>> GetCategoryAnalyticsAsync(
    [Query] Guid profileId,
    [Query] string type,
    [Query] string? dateFrom,
    [Query] string? dateTo,
    CancellationToken ct = default);

  [Get("/api/analytics/monthly")]
  Task<IReadOnlyList<MonthlyAnalyticsDto>> GetMonthlyAnalyticsAsync(
    [Query] Guid profileId,
    [Query] int? year,
    CancellationToken ct = default);
}

public record CategoryAnalyticsDto(Guid? CategoryId, decimal Total, string Color);
public record MonthlyAnalyticsDto(string Month, decimal Income, decimal Expense, decimal CumulativeBalance);
