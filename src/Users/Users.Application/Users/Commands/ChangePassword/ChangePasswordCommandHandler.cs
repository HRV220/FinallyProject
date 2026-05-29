using MediatR;
using Users.Domain.Interfaces;
using Users.Domain.ValueObject;
using Users.Application.Interfaces;
using Core.Domain.Common;

namespace Users.Application.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<bool>>
{
  private readonly IUserRepository _userRepository;
  private readonly IPasswordHasher _passwordHasher;

  public ChangePasswordCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
  {
    _userRepository = userRepository;
    _passwordHasher = passwordHasher;
  }

  public async Task<Result<bool>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetByIdAsync(command.Id);
    if (user is null)
      return Result<bool>.Failure(new DomainError("User.NotFound", "User not found."));

    if (!_passwordHasher.Verify(command.CurrentPassword, user.PasswordHash.Value))
      return Result<bool>.Failure(new DomainError("User.InvalidPassword", "Current password is incorrect."));

    var newHash = _passwordHasher.Hash(command.NewPassword);
    var newPasswordHash = PasswordHash.Create(newHash);
    if (newPasswordHash.IsFailure)
      return Result<bool>.Failure(newPasswordHash.Error!);

    user.ChangeHashPassword(newPasswordHash.Value!);
    await _userRepository.UpdateUserAsync(user);
    return Result<bool>.Success(true);
  }
}
