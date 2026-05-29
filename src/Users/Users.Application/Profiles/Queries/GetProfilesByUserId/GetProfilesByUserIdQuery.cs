using MediatR;
using Core.Domain.Common;
namespace Users.Application.Profiles.Queries.GetProfilesByUserId;
public record GetProfilesByUserIdQuery(Guid UserId) : IRequest<Result<IEnumerable<GetProfilesByUserIdResponse>>>;