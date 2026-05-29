using MediatR;
using Core.Domain.Common;
using Categories.Domain.Entities;
using Categories.Domain.Interfaces;

namespace Categories.Application.Categories.Commands.CreateSystemCategory;

public class CreateSystemCategoryCommandHandler : IRequestHandler<CreateSystemCategoryCommand, Result<Guid>>
{
  private readonly ICategoryRepository _categoryRepository;

  public CreateSystemCategoryCommandHandler(ICategoryRepository categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  public async Task<Result<Guid>> Handle(CreateSystemCategoryCommand command, CancellationToken cancellationToken)
  {
    var (name, type, icon) = command;
    var category = Category.CreateSystem(name, type, icon);

    if (category.IsFailure)
      return Result<Guid>.Failure(category.Error!);

    await _categoryRepository.CreateAsync(category.Value!);
    return Result<Guid>.Success(category.Value!.Id);
  }
}
