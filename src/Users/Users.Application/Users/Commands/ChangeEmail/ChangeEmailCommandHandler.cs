using MediatR;
using Users.Application.Interfaces;
using Core.Domain.Common;
using Users.Domain.Interfaces;
using Users.Domain.ValueObject;

namespace Users.Application.Users.Commands.ChangeEmail;

public class ChangeEmailCommandHandler : IRequestHandler<ChangeEmailCommand, Result<bool>>
{
  private readonly IUserRepository _userRepository;

  public ChangeEmailCommandHandler(IUserRepository userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task<Result<bool>> Handle(ChangeEmailCommand command, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetByIdAsync(command.Id);
    if (user is null)
      return Result<bool>.Failure(new DomainError("User.NotFound", "User not found."));

    var newEmail = Email.Create(command.NewEmail);
    if (newEmail.IsFailure)
      return Result<bool>.Failure(newEmail.Error!);

    var existingUser = await _userRepository.GetByEmailAsync(newEmail.Value!);
    if (existingUser is not null)
      return Result<bool>.Failure(new DomainError("User.EmailAlreadyTaken", "This email is already taken."));

    user.ChangeEmail(newEmail.Value!);
    await _userRepository.UpdateUserAsync(user);
    return Result<bool>.Success(true);
  }
}
