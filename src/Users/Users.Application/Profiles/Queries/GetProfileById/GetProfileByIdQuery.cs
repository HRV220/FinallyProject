using MediatR;
using Core.Domain.Common;
namespace Users.Application.Profiles.Queries.GetProfileById;

public record GetProfileByIdQuery(Guid Id) : IRequest<Result<GetProfileByIdResponse>>;