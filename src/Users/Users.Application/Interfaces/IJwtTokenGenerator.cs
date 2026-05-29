namespace Users.Application.Interfaces;

public interface IJwtTokenGenerator
{
  string GenerateJwtToken(Guid userId, string email);
}