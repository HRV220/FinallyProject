using MediatR;
using Core.Domain.Common;
using Categories.Domain.Enums;

namespace Categories.Application.Categories.Commands.CreateSystemCategory;

public record CreateSystemCategoryCommand(string Name, CategoryType Type, string? Icon = null) : IRequest<Result<Guid>>;
