namespace Transactions.Application.Abstractions;

/// <summary>
/// Проверка существования категории. Может вернуть Unknown при недоступности Categories Service
/// (graceful degradation — транзакция тогда создаётся с categoryId = null).
/// </summary>
public interface ICategoryChecker
{
  Task<CategoryCheckResult> CheckAsync(Guid categoryId, Guid? profileId, CancellationToken ct = default);
}

public enum CategoryCheckResult
{
  /// <summary>Категория существует и доступна владельцу профиля (или системная).</summary>
  Ok,
  /// <summary>Категория не найдена.</summary>
  NotFound,
  /// <summary>Категория принадлежит чужому профилю.</summary>
  Forbidden,
  /// <summary>Categories Service недоступен — graceful degradation.</summary>
  Unknown
}
