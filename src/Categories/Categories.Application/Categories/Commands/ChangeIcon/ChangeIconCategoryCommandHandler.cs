using MediatR;
using Core.Domain.Common;
using Categories.Domain.Interfaces;

namespace Categories.Application.Categories.Commands.ChangeIcon;

public class ChangeIconCategoryCommandHandler : IRequestHandler<ChangeIconCategoryCommand, Result<bool>>
{
  private readonly ICategoryRepository _categoryRepository;

  public ChangeIconCategoryCommandHandler(ICategoryRepository categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  public async Task<Result<bool>> Handle(ChangeIconCategoryCommand command, CancellationToken cancellationToken)
  {
    var category = await _categoryRepository.GetByIdAsync(command.Id);
    if (category is null)
      return Result<bool>.Failure(new DomainError("Category.NotFound", "Category not found."));

    if (category.IsSystem)
      return Result<bool>.Failure(new DomainError("Category.CannotModifySystem", "System category cannot be modified."));

    category.ChangeIcon(command.NewIcon);
    await _categoryRepository.UpdateAsync(category);
    return Result<bool>.Success(true);
  }
}
