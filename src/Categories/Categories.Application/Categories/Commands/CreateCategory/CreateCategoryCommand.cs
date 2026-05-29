using MediatR;
using Core.Domain.Common;
using Categories.Domain.Enums;

namespace Categories.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(Guid ProfileId, string Name, CategoryType Type, string? Icon = null) : IRequest<Result<Guid>>;
