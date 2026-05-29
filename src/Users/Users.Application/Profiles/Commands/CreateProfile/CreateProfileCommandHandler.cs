using MediatR;
using Users.Domain.Entities;
using Users.Domain.Interfaces;
using Core.Domain.Common;

namespace Users.Application.Profiles.Commands.CreateProfile;

public class CreateProfileCommandHandler : IRequestHandler<CreateProfileCommand, Result<Guid>>
{
  private readonly IProfileRepository _profileRepository;
  private readonly IUserRepository _userRepository;

  public CreateProfileCommandHandler(IProfileRepository profileRepository, IUserRepository userRepository)
  {
    _profileRepository = profileRepository;
    _userRepository = userRepository;
  }

  public async Task<Result<Guid>> Handle(CreateProfileCommand command, CancellationToken cancellationToken)
  {
    var existUser = await _userRepository.GetByIdAsync(command.UserId);
    if (existUser == null)
      return Result<Guid>.Failure(new DomainError("Profile.UserNotExist", "A non-existent user when creating a profile"));

    var profileNames = await _profileRepository.GetByUserIdProfilesAsync(command.UserId);
    if (profileNames.Select(e => e.Name).Contains(command.Name))
      return Result<Guid>.Failure(new DomainError("Profile.ExistName", "This name already exists."));

    var profile = Profile.Create(command.Name, command.UserId);

    await _profileRepository.AddProfileAsync(profile);

    return Result<Guid>.Success(profile.Id);
  }
}