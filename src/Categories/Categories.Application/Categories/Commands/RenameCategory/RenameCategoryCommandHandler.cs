using MediatR;
using Core.Domain.Common;
using Categories.Domain.Interfaces;

namespace Categories.Application.Categories.Commands.RenameCategory;

public class RenameCategoryCommandHandler : IRequestHandler<RenameCategoryCommand, Result<bool>>
{
  private readonly ICategoryRepository _categoryRepository;

  public RenameCategoryCommandHandler(ICategoryRepository categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  public async Task<Result<bool>> Handle(RenameCategoryCommand command, CancellationToken cancellationToken)
  {
    var category = await _categoryRepository.GetByIdAsync(command.Id);
    if (category is null)
      return Result<bool>.Failure(new DomainError("Category.NotFound", "Category not found."));

    if (category.IsSystem)
      return Result<bool>.Failure(new DomainError("Category.CannotModifySystem", "System category cannot be modified."));

    var result = category.Rename(command.NewName);
    if (result.IsFailure)
      return result;

    await _categoryRepository.UpdateAsync(category);
    return Result<bool>.Success(true);
  }
}
