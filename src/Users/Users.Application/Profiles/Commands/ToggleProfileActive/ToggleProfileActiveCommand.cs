using MediatR;
using Core.Domain.Common;
namespace Users.Application.Profiles.Commands.ToggleProfileActive;

public record ToggleProfileActiveCommand(Guid Id) : IRequest<Result<bool>>;
