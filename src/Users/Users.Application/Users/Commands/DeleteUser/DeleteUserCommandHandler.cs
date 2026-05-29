using MediatR;
using Users.Domain.Interfaces;
using Core.Domain.Common;

namespace Users.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<bool>>
{
  private readonly IUserRepository _userRepository;

  public DeleteUserCommandHandler(IUserRepository userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task<Result<bool>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetByIdAsync(command.Id);
    if (user is null)
      return Result<bool>.Failure(new DomainError("User.NotFound", "User not found."));

    await _userRepository.DeleteUserAsync(user);
    return Result<bool>.Success(true);
  }
}
