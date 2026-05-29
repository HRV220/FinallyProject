using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Refit;
using Reports.Application.Abstractions;
using Reports.Domain.Entities;
using Reports.Domain.Enums;
using Reports.Infrastructure.Clients;

namespace Reports.Infrastructure.Generation;

/// <summary>
/// Stage 4 (минимально): генерируем CSV вместо PDF — нет внешней зависимости от PDF-движка.
/// Реальный CategoryName резолвится из Categories Service (graceful: если недоступен → CategoryId).
/// </summary>
public class CsvReportGenerator : IReportGenerator
{
  private static readonly JsonSerializerOptions JsonOptions = new()
  {
    PropertyNameCaseInsensitive = true
  };

  private readonly IUsersServiceClient _users;
  private readonly ICategoriesServiceClient _categories;
  private readonly IAnalyticsServiceClient _analytics;
  private readonly ILogger<CsvReportGenerator> _logger;

  public CsvReportGenerator(
    IUsersServiceClient users,
    ICategoriesServiceClient categories,
    IAnalyticsServiceClient analytics,
    ILogger<CsvReportGenerator> logger)
  {
    _users = users;
    _categories = categories;
    _analytics = analytics;
    _logger = logger;
  }

  public async Task<GeneratedReport> GenerateAsync(ReportJob job, CancellationToken ct = default)
  {
    var parameters = JsonSerializer.Deserialize<ReportParameters>(job.ParametersJson, JsonOptions)
      ?? throw new InvalidOperationException("Report parameters JSON is invalid.");

    if (parameters.ProfileId == Guid.Empty)
      throw new InvalidOperationException("ProfileId is required in report parameters.");

    var profile = await TryGetProfileAsync(parameters.ProfileId, ct);
    var categoriesMap = await TryGetCategoriesMapAsync(parameters.ProfileId, ct);

    var content = job.Type switch
    {
      ReportType.ProfileTransactions => await BuildProfileTransactionsAsync(profile, parameters, ct),
      ReportType.CategoryBreakdown => await BuildCategoryBreakdownAsync(profile, parameters, categoriesMap, ct),
      _ => throw new InvalidOperationException($"Unsupported report type: {job.Type}.")
    };

    var key = $"{job.Id:N}.csv";
    var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(content)).ToArray();
    return new GeneratedReport(key, bytes, "text/csv; charset=utf-8");
  }

  private async Task<string> BuildProfileTransactionsAsync(ProfileDto? profile, ReportParameters p, CancellationToken ct)
  {
    var monthly = await _analytics.GetMonthlyAnalyticsAsync(p.ProfileId, p.From?.Year ?? DateTime.UtcNow.Year, ct);

    var sb = new StringBuilder();
    sb.AppendLine($"# Profile Transactions Report");
    sb.AppendLine($"# Profile: {profile?.Name ?? p.ProfileId.ToString()}");
    sb.AppendLine($"# Period: {p.From:yyyy-MM-dd} .. {p.To:yyyy-MM-dd}");
    sb.AppendLine($"# Generated: {DateTime.UtcNow:O}");
    sb.AppendLine();
    sb.AppendLine("Month;Income;Expense;CumulativeBalance");

    foreach (var row in monthly)
      sb.AppendLine(string.Join(';',
        row.Month,
        row.Income.ToString(CultureInfo.InvariantCulture),
        row.Expense.ToString(CultureInfo.InvariantCulture),
        row.CumulativeBalance.ToString(CultureInfo.InvariantCulture)));

    return sb.ToString();
  }

  private async Task<string> BuildCategoryBreakdownAsync(
    ProfileDto? profile,
    ReportParameters p,
    IReadOnlyDictionary<Guid, string> categoriesMap,
    CancellationToken ct)
  {
    var rows = await _analytics.GetCategoryAnalyticsAsync(
      p.ProfileId,
      "Expense",
      p.From?.ToString("yyyy-MM-dd"),
      p.To?.ToString("yyyy-MM-dd"),
      ct);

    var sb = new StringBuilder();
    sb.AppendLine($"# Category Breakdown Report (Expense)");
    sb.AppendLine($"# Profile: {profile?.Name ?? p.ProfileId.ToString()}");
    sb.AppendLine($"# Period: {p.From:yyyy-MM-dd} .. {p.To:yyyy-MM-dd}");
    sb.AppendLine($"# Generated: {DateTime.UtcNow:O}");
    sb.AppendLine();
    sb.AppendLine("CategoryId;CategoryName;Total");

    foreach (var row in rows)
    {
      var name = row.CategoryId.HasValue && categoriesMap.TryGetValue(row.CategoryId.Value, out var n)
        ? n
        : "<uncategorized>";
      sb.AppendLine(string.Join(';',
        row.CategoryId?.ToString() ?? string.Empty,
        Escape(name),
        row.Total.ToString(CultureInfo.InvariantCulture)));
    }

    return sb.ToString();
  }

  private async Task<ProfileDto?> TryGetProfileAsync(Guid profileId, CancellationToken ct)
  {
    try
    {
      return await _users.GetProfileAsync(profileId, ct);
    }
    catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
      _logger.LogWarning("Profile {ProfileId} not found at Users Service.", profileId);
      return null;
    }
    catch (Exception ex)
    {
      // Graceful degradation: имя профиля — это украшение отчёта.
      _logger.LogWarning(ex, "Users Service unavailable while generating report (profile={ProfileId}).", profileId);
      return null;
    }
  }

  private async Task<IReadOnlyDictionary<Guid, string>> TryGetCategoriesMapAsync(Guid profileId, CancellationToken ct)
  {
    try
    {
      var list = await _categories.GetCategoriesByProfileAsync(profileId, ct);
      return list.ToDictionary(c => c.Id, c => c.Name);
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "Categories Service unavailable while generating report (profile={ProfileId}). Names will be missing.", profileId);
      return new Dictionary<Guid, string>();
    }
  }

  private static string Escape(string value)
  {
    if (value.Contains(';') || value.Contains('"') || value.Contains('\n'))
      return "\"" + value.Replace("\"", "\"\"") + "\"";
    return value;
  }

  private sealed record ReportParameters(Guid ProfileId, DateOnly? From, DateOnly? To);
}
