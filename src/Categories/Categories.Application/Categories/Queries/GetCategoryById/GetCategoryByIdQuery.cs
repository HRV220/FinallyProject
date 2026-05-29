using MediatR;
using Core.Domain.Common;
namespace Categories.Application.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<Result<GetCategoryByIdResponse>>;
