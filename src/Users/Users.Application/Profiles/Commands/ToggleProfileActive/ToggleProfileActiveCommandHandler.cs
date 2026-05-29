using MediatR;
using Core.Domain.Common;
using Users.Domain.Interfaces;

namespace Users.Application.Profiles.Commands.ToggleProfileActive;

public class ToggleProfileActiveCommandHandler : IRequestHandler<ToggleProfileActiveCommand, Result<bool>>
{
  private readonly IProfileRepository _profileRepository;

  public ToggleProfileActiveCommandHandler(IProfileRepository profileRepository)
  {
    _profileRepository = profileRepository;
  }

  public async Task<Result<bool>> Handle(ToggleProfileActiveCommand command, CancellationToken cancellationToken)
  {
    var profile = await _profileRepository.GetByIdProfileAsync(command.Id);
    if (profile is null)
      return Result<bool>.Failure(new DomainError("Profile.NotFound", "Profile not found."));

    profile.ActivateDeactivate();
    await _profileRepository.UpdateProfileAsync(profile);
    return Result<bool>.Success(true);
  }
}
