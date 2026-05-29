using MediatR;
using Core.Domain.Common;
namespace Users.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<Result<GetUserByIdResponse>>;
