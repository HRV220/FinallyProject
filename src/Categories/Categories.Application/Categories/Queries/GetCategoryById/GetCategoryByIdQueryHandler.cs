using MediatR;
using Core.Domain.Common;
using Categories.Domain.Interfaces;

namespace Categories.Application.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<GetCategoryByIdResponse>>
{
  private readonly ICategoryRepository _categoryRepository;

  public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  public async Task<Result<GetCategoryByIdResponse>> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
  {
    var category = await _categoryRepository.GetByIdAsync(query.Id);
    if (category is null)
      return Result<GetCategoryByIdResponse>.Failure(new DomainError("Category.NotFound", "Category not found."));

    return Result<GetCategoryByIdResponse>.Success(new GetCategoryByIdResponse(
      category.Id,
      category.Name,
      category.Type,
      category.Icon,
      category.IsSystem,
      category.ProfileId));
  }
}
