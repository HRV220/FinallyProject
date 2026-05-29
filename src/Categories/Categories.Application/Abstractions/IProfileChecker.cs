namespace Categories.Application.Abstractions;

/// <summary>
/// Проверка существования профиля. На Этапе 3 — локальная заглушка;
/// на Этапе 4 будет HTTP-вызов к Users Service.
/// </summary>
public interface IProfileChecker
{
  Task<bool> ExistsAsync(Guid profileId, CancellationToken ct = default);
}
