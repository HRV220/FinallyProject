using MediatR;
using Core.Domain.Common;
using Users.Domain.Interfaces;

namespace Users.Application.Profiles.Queries.GetProfilesByUserId;
public class GetProfilesByUserIdQueryHandler : IRequestHandler<GetProfilesByUserIdQuery, Result<IEnumerable<GetProfilesByUserIdResponse>>>
{
  private readonly IProfileRepository _profileRepository;

  public GetProfilesByUserIdQueryHandler(IProfileRepository profileRepository)
  {
    _profileRepository = profileRepository;
  }

  public async Task<Result<IEnumerable<GetProfilesByUserIdResponse>>> Handle(GetProfilesByUserIdQuery query, CancellationToken cancellationToken)
  {
    var profiles = await _profileRepository.GetByUserIdProfilesAsync(query.UserId);
    var response = profiles.Select(p => new GetProfilesByUserIdResponse(p.Id, p.Name, p.IsActive));
    return Result<IEnumerable<GetProfilesByUserIdResponse>>.Success(response);
  }
}