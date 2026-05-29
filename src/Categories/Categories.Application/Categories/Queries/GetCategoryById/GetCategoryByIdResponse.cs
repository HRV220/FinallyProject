using Categories.Domain.Enums;

namespace Categories.Application.Categories.Queries.GetCategoryById;

public record GetCategoryByIdResponse(
  Guid Id,
  string Name,
  CategoryType Type,
  string? Icon,
  bool IsSystem,
  Guid? ProfileId);
