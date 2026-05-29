namespace Core.Domain.Entities;

public abstract class BaseEntity
{
  public Guid Id { get; protected set; }

  public DateTime CreatedAt { get; protected set; }

  public DateTime? UpdatedAt { get; protected set; } = null;

  protected BaseEntity()
  {

  }

  protected BaseEntity(Guid id)
  {
    if (id == Guid.Empty)
      throw new ArgumentException("ID cannot be empty", nameof(id));
    Id = id;
    CreatedAt = DateTime.UtcNow;
  }

  protected void SetUpdated()
  {
    UpdatedAt = DateTime.UtcNow;
  }

  public override bool Equals(object? obj)
  {
    if (obj is not BaseEntity other)
      return false;
    if (GetType() != other.GetType()) return false; // строгое сравнение типа
    return Id == other.Id;
  }

  public override int GetHashCode()
  {
    return Id.GetHashCode();
  }

  public static bool operator ==(BaseEntity? left, BaseEntity? right)
  {
    if (left is null && right is null) return true;
    if (left is null || right is null) return false;
    return left.Equals(right);
  }

  public static bool operator !=(BaseEntity? left, BaseEntity? right) => !(left == right);
}
