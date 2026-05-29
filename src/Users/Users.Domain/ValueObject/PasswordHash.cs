using Core.Domain.Common;

namespace Users.Domain.ValueObject;

public sealed record PasswordHash
{
  public string Value { get; }

  private PasswordHash(string value)
  {
    Value = value;
  }

  public static Result<PasswordHash> Create(string value)
  {
    if (string.IsNullOrEmpty(value))
      return Result<PasswordHash>.Failure(new DomainError("PasswordHash.Empty", "Password hash can't be empty!"));
    return Result<PasswordHash>.Success(new PasswordHash(value));
  }
}