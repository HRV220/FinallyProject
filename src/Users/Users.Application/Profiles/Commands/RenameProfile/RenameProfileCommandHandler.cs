using MediatR;
using Core.Domain.Common;
using Users.Domain.Interfaces;

namespace Users.Application.Profiles.Commands.RenameProfile;

public class RenameProfileCommandHandler : IRequestHandler<RenameProfileCommand, Result<bool>>
{
  private readonly IProfileRepository _profileRepository;

  public RenameProfileCommandHandler(IProfileRepository profileRepository)
  {
    _profileRepository = profileRepository;
  }

  public async Task<Result<bool>> Handle(RenameProfileCommand command, CancellationToken cancellationToken)
  {
    var profile = await _profileRepository.GetByIdProfileAsync(command.Id);
    if (profile is null)
      return Result<bool>.Failure(new DomainError("Profile.NotFound", "Profile not found."));

    profile.Rename(command.NewName);
    await _profileRepository.UpdateProfileAsync(profile);
    return Result<bool>.Success(true);
  }
}
