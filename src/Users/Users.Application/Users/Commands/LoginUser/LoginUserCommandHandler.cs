using MediatR;
using Users.Domain.Interfaces;
using Users.Domain.ValueObject;
using Users.Application.Interfaces;
using Core.Domain.Common;


namespace Users.Application.Users.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<LoginUserResponse>>
{
  private readonly IUserRepository _userRepository;
  private readonly IPasswordHasher _passwordHasher;
  private readonly IJwtTokenGenerator _jwtTokenGenerator;

  public LoginUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
  {
    _jwtTokenGenerator = jwtTokenGenerator;
    _passwordHasher = passwordHasher;
    _userRepository = userRepository;
  }

  public async Task<Result<LoginUserResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
  {
    var email = Email.Create(command.Email);
    if (email.IsFailure)
      return Result<LoginUserResponse>.Failure(email.Error!);
    var user = await _userRepository.GetByEmailAsync(email.Value!);
    if (user == null)
      return Result<LoginUserResponse>.Failure(new DomainError("User.NotFound", "User not found."));
    if (!_passwordHasher.Verify(command.Password, user.PasswordHash.Value!))
      return Result<LoginUserResponse>.Failure(new DomainError("User.InvalidPassword", "Invalid password"));

    var jwtToken = _jwtTokenGenerator.GenerateJwtToken(user.Id, user.Email.Value!);
    return Result<LoginUserResponse>.Success(new LoginUserResponse(user.Id, jwtToken));
  }
}
