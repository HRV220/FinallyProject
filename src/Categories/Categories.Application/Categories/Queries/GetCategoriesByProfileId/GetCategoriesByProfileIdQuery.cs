using MediatR;
using Core.Domain.Common;
namespace Categories.Application.Categories.Queries.GetCategoriesByProfileId;

public record GetCategoriesByProfileIdQuery(Guid ProfileId) : IRequest<Result<IEnumerable<GetCategoriesByProfileIdResponse>>>;
