using Categories.Application.Abstractions;
using Categories.Infrastructure.Clients;
using Microsoft.Extensions.Logging;
using Refit;

namespace Categories.Infrastructure.Services;

/// <summary>
/// Stage 4: проверка профиля через HTTP-вызов к Users Service.
/// Возвращает true только если профиль существует и активен.
/// </summary>
public class HttpProfileChecker : IProfileChecker
{
  private readonly IUsersServiceClient _users;
  private readonly ILogger<HttpProfileChecker> _logger;

  public HttpProfileChecker(IUsersServiceClient users, ILogger<HttpProfileChecker> logger)
  {
    _users = users;
    _logger = logger;
  }

  public async Task<bool> ExistsAsync(Guid profileId, CancellationToken ct = default)
  {
    if (profileId == Guid.Empty) return false;

    try
    {
      var profile = await _users.GetProfileAsync(profileId, ct);
      return profile.IsActive;
    }
    catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
      return false;
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "Users Service недоступен при проверке профиля {ProfileId}", profileId);
      throw new UsersUnavailableException("Users Service недоступен.", ex);
    }
  }
}

