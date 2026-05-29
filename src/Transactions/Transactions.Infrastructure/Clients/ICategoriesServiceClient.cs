using Refit;

namespace Transactions.Infrastructure.Clients;

public interface ICategoriesServiceClient
{
  [Get("/api/categories/{id}")]
  Task<CategoryDto> GetCategoryAsync(Guid id, CancellationToken ct = default);
}

public record CategoryDto(Guid Id, string Name, string Type, string? Icon, bool IsSystem, Guid? ProfileId);
