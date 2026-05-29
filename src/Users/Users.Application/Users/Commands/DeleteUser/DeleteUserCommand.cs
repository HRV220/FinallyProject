using MediatR;
using Core.Domain.Common;
namespace Users.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest<Result<bool>>;
