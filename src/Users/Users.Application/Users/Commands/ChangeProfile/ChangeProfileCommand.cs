using MediatR;
using Core.Domain.Common;
namespace Users.Application.Users.Commands.ChangeProfile;

public record ChangeProfileCommand(Guid UserId, string? FirstName, string? LastName, string? PhoneNumber) : IRequest<Result<bool>>;
