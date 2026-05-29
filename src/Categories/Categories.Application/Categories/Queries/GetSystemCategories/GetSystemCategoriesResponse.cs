using Categories.Domain.Enums;

namespace Categories.Application.Categories.Queries.GetSystemCategories;

public record GetSystemCategoriesResponse(Guid Id, string Name, CategoryType Type, string? Icon);
