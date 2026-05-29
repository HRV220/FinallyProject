using Core.Domain.Common;

namespace Users.Domain.ValueObject;
public sealed record Email
{
  public string Value { get; }

  private Email(string value)
  {
    Value = value;
  }

  public static Result<Email> Create(string value)
  {
    if (string.IsNullOrEmpty(value))
      return Result<Email>.Failure(new DomainError("Email.Empty", "Email can't be empty."));
    string normalized_value = value.Trim().ToLower();

    if (!normalized_value.Contains('@'))
      return Result<Email>.Failure(new DomainError("Email.InvalidFormat", "Email don't contain specialize symbol - @"));

    if (normalized_value.Length > 254)
      return Result<Email>.Failure(new DomainError("Email.TooLong", "Email too long"));

    return Result<Email>.Success(new Email(normalized_value));
  }

  public override string ToString() => Value;
}