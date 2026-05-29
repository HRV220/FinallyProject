using MediatR;
using Core.Domain.Common;
using Users.Domain.Interfaces;

namespace Users.Application.Profiles.Queries.GetProfileById;

public class GetProfileByIdQueryHandler : IRequestHandler<GetProfileByIdQuery, Result<GetProfileByIdResponse>>
{
  private readonly IProfileRepository _profileRepository;

  public GetProfileByIdQueryHandler(IProfileRepository profileRepository)
  {
    _profileRepository = profileRepository;
  }

  public async Task<Result<GetProfileByIdResponse>> Handle(GetProfileByIdQuery query, CancellationToken cancellationToken)
  {
    var profile = await _profileRepository.GetByIdProfileAsync(query.Id);
    if (profile == null)
      return Result<GetProfileByIdResponse>.Failure(new DomainError("Profile.NotFound", "Profile not found."));

    return Result<GetProfileByIdResponse>.Success(new GetProfileByIdResponse(profile.Id, profile.UserId, profile.Name, profile.IsActive));
  }
}