using MediatR;
using Core.Domain.Common;
namespace Categories.Application.Categories.Commands.ChangeIcon;

public record ChangeIconCategoryCommand(Guid Id, string? NewIcon) : IRequest<Result<bool>>;
