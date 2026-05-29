using MediatR;
using Core.Domain.Common;
namespace Users.Application.Users.Commands.RegisterUser;
public record RegisterUserCommand(string Email, string Password) : IRequest<Result<Guid>>;
