using MediatR;
using Core.Domain.Common;
namespace Categories.Application.Categories.Queries.GetSystemCategories;

public record GetSystemCategoriesQuery() : IRequest<Result<IEnumerable<GetSystemCategoriesResponse>>>;
