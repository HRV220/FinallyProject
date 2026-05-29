using MediatR;
using Core.Domain.Common;
using Users.Domain.Interfaces;

namespace Users.Application.Users.Commands.ChangeProfile;

public class ChangeProfileCommandHandler : IRequestHandler<ChangeProfileCommand, Result<bool>>
{
  private readonly IUserRepository _userRepository;

  public ChangeProfileCommandHandler(IUserRepository userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task<Result<bool>> Handle(ChangeProfileCommand command, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetByIdAsync(command.UserId);
    if (user is null)
      return Result<bool>.Failure(new DomainError("User.NotFound", "User not found."));

    user.ChangeProfile(command.FirstName, command.LastName, command.PhoneNumber);
    await _userRepository.UpdateUserAsync(user);
    return Result<bool>.Success(true);
  }
}
