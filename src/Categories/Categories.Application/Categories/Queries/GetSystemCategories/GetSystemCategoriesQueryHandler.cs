using MediatR;
using Core.Domain.Common;
using Categories.Domain.Interfaces;

namespace Categories.Application.Categories.Queries.GetSystemCategories;

public class GetSystemCategoriesQueryHandler : IRequestHandler<GetSystemCategoriesQuery, Result<IEnumerable<GetSystemCategoriesResponse>>>
{
  private readonly ICategoryRepository _categoryRepository;

  public GetSystemCategoriesQueryHandler(ICategoryRepository categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  public async Task<Result<IEnumerable<GetSystemCategoriesResponse>>> Handle(GetSystemCategoriesQuery query, CancellationToken cancellationToken)
  {
    var categories = await _categoryRepository.GetSystemCategoriesAsync();

    var response = categories.Select(c => new GetSystemCategoriesResponse(c.Id, c.Name, c.Type, c.Icon));

    return Result<IEnumerable<GetSystemCategoriesResponse>>.Success(response);
  }
}
