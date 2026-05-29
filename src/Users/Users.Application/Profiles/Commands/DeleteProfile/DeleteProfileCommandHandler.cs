using MediatR;
using Core.Domain.Common;
using Users.Domain.Interfaces;

namespace Users.Application.Profiles.Commands.DeleteProfile;

public class DeleteProfileCommandHandler : IRequestHandler<DeleteProfileCommand, Result<bool>>
{
  private readonly IProfileRepository _profileRepository;

  public DeleteProfileCommandHandler(IProfileRepository profileRepository)
  {
    _profileRepository = profileRepository;
  }

  public async Task<Result<bool>> Handle(DeleteProfileCommand command, CancellationToken cancellationToken)
  {
    var profile = await _profileRepository.GetByIdProfileAsync(command.Id);
    if (profile is null)
      return Result<bool>.Failure(new DomainError("Profile.NotFound", "Profile not found."));

    await _profileRepository.DeleteAsync(profile);
    return Result<bool>.Success(true);
  }
}
