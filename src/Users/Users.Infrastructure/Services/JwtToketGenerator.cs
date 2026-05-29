using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Users.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Users.Infrastructure.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
  private readonly IConfiguration _configuration;

  public JwtTokenGenerator(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public string GenerateJwtToken(Guid userId, string email)
  {
    var claims = new List<Claim>
    {
      new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
      new Claim(JwtRegisteredClaimNames.Email, email),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
      new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
    };
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      issuer: _configuration["Jwt:Issuer"],
      audience: _configuration["Jwt:Audience"],
      claims: claims,
      expires: DateTime.UtcNow.AddHours(_configuration.GetValue<double>("Jwt:ExpirationHours", 1.0)),
      signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}