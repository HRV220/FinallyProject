using MediatR;
using Core.Domain.Common;
namespace Users.Application.Users.Commands.ChangeEmail;

public record ChangeEmailCommand(Guid Id, string NewEmail) : IRequest<Result<bool>>;
