using MediatR;
using Core.Domain.Common;
namespace Users.Application.Users.Commands.ChangePassword;

public record ChangePasswordCommand(Guid Id, string CurrentPassword, string NewPassword) : IRequest<Result<bool>>;
