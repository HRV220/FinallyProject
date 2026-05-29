using Core.Domain.Entities;
using Users.Domain.ValueObject;

namespace Users.Domain.Entities;

public class User : BaseEntity
{
  public Email Email { get; private set; } = null!;
  public PasswordHash PasswordHash { get; private set; } = null!;
  public string? FirstName { get; private set; }
  public string? LastName { get; private set; }
  public string? PhoneNumber { get; private set; }

  private readonly List<Profile> _profiles = [];
  public IReadOnlyCollection<Profile> Profiles => _profiles.AsReadOnly();

  private User() : base()
  {

  }

  private User(Guid id, Email email, PasswordHash passwordHash) : base(id)
  {
    Email = email;
    PasswordHash = passwordHash;
  }

  public static User Create(Email email, PasswordHash passwordHash)
  {
    return new User(Guid.NewGuid(), email, passwordHash);
  }

  public void ChangeEmail(Email newEmail)
  {
    Email = newEmail;
    SetUpdated();
  }

  public void ChangeHashPassword(PasswordHash newHashPassword)
  {
    PasswordHash = newHashPassword;
    SetUpdated();
  }

  public void ChangeProfile(string? firstName, string? lastName, string? phoneNumber)
  {
    FirstName = firstName;
    LastName = lastName;
    PhoneNumber = phoneNumber;
    SetUpdated();
  }

}