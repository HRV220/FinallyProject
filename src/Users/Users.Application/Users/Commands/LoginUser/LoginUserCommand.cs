using MediatR;
using Core.Domain.Common;
namespace Users.Application.Users.Commands.LoginUser;
public record LoginUserCommand(string Email, string Password) : IRequest<Result<LoginUserResponse>>;