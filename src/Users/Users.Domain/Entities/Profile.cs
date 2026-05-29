using Core.Domain.Entities;

namespace Users.Domain.Entities;

public class Profile : BaseEntity
{
  public Guid UserId { get; private set; }
  public string Name { get; private set; } = string.Empty;
  public bool IsActive { get; private set; } = true;
  public User User { get; private set; } = null!;

  private Profile() : base() { }

  private Profile(Guid id, Guid userId, string name) : base(id)
  {
    UserId = userId;
    Name = name;
  }

  public static Profile Create(string name, Guid userId)
  {
    return new Profile(Guid.NewGuid(), userId, name);
  }

  public void Rename(string newName)
  {
    Name = newName;
    SetUpdated();
  }

  public void ActivateDeactivate()
  {
    IsActive = !IsActive;
    SetUpdated();
  }
}
