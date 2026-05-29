using MediatR;
using Core.Domain.Common;
using Categories.Domain.Interfaces;

namespace Categories.Application.Categories.Queries.GetCategoriesByProfileId;

public class GetCategoriesByProfileIdQueryHandler : IRequestHandler<GetCategoriesByProfileIdQuery, Result<IEnumerable<GetCategoriesByProfileIdResponse>>>
{
  private readonly ICategoryRepository _categoryRepository;

  public GetCategoriesByProfileIdQueryHandler(ICategoryRepository categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  public async Task<Result<IEnumerable<GetCategoriesByProfileIdResponse>>> Handle(GetCategoriesByProfileIdQuery query, CancellationToken cancellationToken)
  {
    var categories = await _categoryRepository.GetByProfileIdAsync(query.ProfileId);

    var response = categories.Select(c => new GetCategoriesByProfileIdResponse(
      c.Id, c.Name, c.Type, c.Icon, c.IsSystem, c.ProfileId));

    return Result<IEnumerable<GetCategoriesByProfileIdResponse>>.Success(response);
  }
}
