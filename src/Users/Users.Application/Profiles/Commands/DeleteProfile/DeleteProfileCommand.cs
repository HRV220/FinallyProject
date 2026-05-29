using MediatR;
using Core.Domain.Common;
namespace Users.Application.Profiles.Commands.DeleteProfile;

public record DeleteProfileCommand(Guid Id) : IRequest<Result<bool>>;
