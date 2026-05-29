using Microsoft.Extensions.Logging;
using Refit;
using Transactions.Application.Abstractions;
using Transactions.Infrastructure.Clients;

namespace Transactions.Infrastructure.Services;

public class HttpCategoryChecker : ICategoryChecker
{
  private readonly ICategoriesServiceClient _categories;
  private readonly ILogger<HttpCategoryChecker> _logger;

  public HttpCategoryChecker(ICategoriesServiceClient categories, ILogger<HttpCategoryChecker> logger)
  {
    _categories = categories;
    _logger = logger;
  }

  public async Task<CategoryCheckResult> CheckAsync(Guid categoryId, Guid? profileId, CancellationToken ct = default)
  {
    if (categoryId == Guid.Empty)
      return CategoryCheckResult.NotFound;

    try
    {
      var category = await _categories.GetCategoryAsync(categoryId, ct);

      if (category.IsSystem) return CategoryCheckResult.Ok;

      if (profileId.HasValue && category.ProfileId != profileId.Value)
        return CategoryCheckResult.Forbidden;

      return CategoryCheckResult.Ok;
    }
    catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
      return CategoryCheckResult.NotFound;
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "Categories Service недоступен при проверке категории {CategoryId} — graceful degradation", categoryId);
      return CategoryCheckResult.Unknown;
    }
  }
}
