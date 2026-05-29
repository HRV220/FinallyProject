using Categories.Domain.Enums;
using Core.Domain.Common;
using Core.Domain.Entities;

namespace Categories.Domain.Entities;

public class Category : BaseEntity
{
  public string Name { get; private set; } = string.Empty;
  public CategoryType Type { get; private set; }
  public string? Icon { get; private set; }
  public bool IsSystem { get; private set; } = false;
  public Guid? ProfileId { get; private set; }

  private Category() : base() { }

  private Category(Guid id, string name, CategoryType type, string? icon, bool isSystem, Guid? profileId) : base(id)
  {
    Name = name;
    Type = type;
    Icon = icon;
    IsSystem = isSystem;
    ProfileId = profileId;
  }

  public static Result<Category> Create(string name, CategoryType type, Guid profileId, string? icon = null)
  {
    var nameValidation = ValidateName(name);
    if (nameValidation.IsFailure)
      return Result<Category>.Failure(nameValidation.Error!);

    if (profileId == Guid.Empty)
      return Result<Category>.Failure(new DomainError("Category.InvalidProfileId", "ProfileId is required."));

    return Result<Category>.Success(new Category(Guid.NewGuid(), name, type, icon, isSystem: false, profileId));
  }

  public static Result<Category> CreateSystem(string name, CategoryType type, string? icon = null)
  {
    var nameValidation = ValidateName(name);
    if (nameValidation.IsFailure)
      return Result<Category>.Failure(nameValidation.Error!);

    return Result<Category>.Success(new Category(Guid.NewGuid(), name, type, icon, isSystem: true, profileId: null));
  }

  public Result<bool> Rename(string newName)
  {
    var nameValidation = ValidateName(newName);
    if (nameValidation.IsFailure)
      return Result<bool>.Failure(nameValidation.Error!);

    Name = newName;
    SetUpdated();
    return Result<bool>.Success(true);
  }

  public void ChangeIcon(string? newIcon)
  {
    Icon = newIcon;
    SetUpdated();
  }

  private static Result<bool> ValidateName(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      return Result<bool>.Failure(new DomainError("Category.InvalidName", "Name is required."));
    if (name.Length > 100)
      return Result<bool>.Failure(new DomainError("Category.InvalidName", "Name must be 100 characters or less."));
    return Result<bool>.Success(true);
  }
}
