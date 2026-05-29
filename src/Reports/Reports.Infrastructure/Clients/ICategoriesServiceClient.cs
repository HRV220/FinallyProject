using Refit;

namespace Reports.Infrastructure.Clients;

public interface ICategoriesServiceClient
{
  [Get("/api/categories")]
  Task<IReadOnlyList<CategoryDto>> GetCategoriesByProfileAsync([Query] Guid profileId, CancellationToken ct = default);
}

public record CategoryDto(Guid Id, Guid ProfileId, string Name, string Type, string? Icon, bool IsBlocked);
