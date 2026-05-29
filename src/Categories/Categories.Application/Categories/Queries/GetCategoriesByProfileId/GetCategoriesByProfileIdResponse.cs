using Categories.Domain.Enums;

namespace Categories.Application.Categories.Queries.GetCategoriesByProfileId;

public record GetCategoriesByProfileIdResponse(
  Guid Id,
  string Name,
  CategoryType Type,
  string? Icon,
  bool IsSystem,
  Guid? ProfileId);
