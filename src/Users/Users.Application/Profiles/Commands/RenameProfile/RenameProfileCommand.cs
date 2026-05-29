using MediatR;
using Core.Domain.Common;
namespace Users.Application.Profiles.Commands.RenameProfile;

public record RenameProfileCommand(Guid Id, string NewName) : IRequest<Result<bool>>;
