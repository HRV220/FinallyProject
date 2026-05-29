using Users.Application.Interfaces;
using BCrypt.Net;

namespace Users.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
  public string Hash(string password)
  {
    string hash = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    return hash;
  }

  public bool Verify(string password, string hash)
  {
    return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
  }
}