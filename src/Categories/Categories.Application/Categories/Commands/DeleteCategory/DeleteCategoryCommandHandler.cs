using MediatR;
using Core.Domain.Common;
using Categories.Domain.Interfaces;

namespace Categories.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result<bool>>
{
  private readonly ICategoryRepository _categoryRepository;

  public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  public async Task<Result<bool>> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
  {
    var category = await _categoryRepository.GetByIdAsync(command.Id);
    if (category is null)
      return Result<bool>.Failure(new DomainError("Category.NotFound", "Category not found."));

    if (category.IsSystem)
      return Result<bool>.Failure(new DomainError("Category.CannotDeleteSystem", "System category cannot be deleted."));

    await _categoryRepository.DeleteAsync(category);
    return Result<bool>.Success(true);
  }
}
