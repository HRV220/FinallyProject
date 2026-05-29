using MediatR;
using Users.Domain.Interfaces;
using Core.Domain.Common;

namespace Users.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<GetUserByIdResponse>>
{
  private readonly IUserRepository _userRepository;

  public GetUserByIdQueryHandler(IUserRepository userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task<Result<GetUserByIdResponse>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetByIdAsync(query.Id);
    if (user is null)
      return Result<GetUserByIdResponse>.Failure(new DomainError("User.NotFound", "User not found."));

    return Result<GetUserByIdResponse>.Success(new GetUserByIdResponse(user.Id, user.Email.Value, user.FirstName, user.LastName, user.PhoneNumber));
  }
}
