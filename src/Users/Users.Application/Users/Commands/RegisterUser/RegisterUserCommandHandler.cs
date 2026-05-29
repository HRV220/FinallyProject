using MediatR;
using Users.Application.Interfaces;
using Users.Domain.Interfaces;
using Users.Domain.ValueObject;
using Users.Domain.Entities;
using Core.Domain.Common;

namespace Users.Application.Users.Commands.RegisterUser;
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
  private readonly IUserRepository _userRepository;
  private readonly IPasswordHasher _passwordHasher;
  public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
  {
    _userRepository = userRepository;
    _passwordHasher = passwordHasher;
  }

  public async Task<Result<Guid>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
  {
    Result<Email> email = Email.Create(command.Email);
    if (email.IsFailure)
    {
      return Result<Guid>.Failure(email.Error!);
    }
    var existingUser = await _userRepository.GetByEmailAsync(email.Value!);

    if (existingUser != null)
    {
      return Result<Guid>.Failure(new DomainError("User.AlreadyExist", "User with this email has already been created"));
    }
    var hash = _passwordHasher.Hash(command.Password);
    Result<PasswordHash> passHash = PasswordHash.Create(hash);
    if (passHash.IsFailure)
    {
      return Result<Guid>.Failure(passHash.Error!);
    }

    var user = User.Create(email.Value!, passHash.Value!);

    await _userRepository.AddUserAsync(user);

    return Result<Guid>.Success(user.Id);

  }
}