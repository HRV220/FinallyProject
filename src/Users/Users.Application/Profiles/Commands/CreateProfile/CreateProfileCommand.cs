using MediatR;
using Core.Domain.Common;
namespace Users.Application.Profiles.Commands.CreateProfile;

public record CreateProfileCommand(Guid UserId, string Name) : IRequest<Result<Guid>>;

