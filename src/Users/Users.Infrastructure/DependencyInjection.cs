using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.Interfaces;
using Users.Domain.Interfaces;
using Users.Infrastructure.Persistence;
using Users.Infrastructure.Persistence.Repositories;
using Users.Infrastructure.Services;

namespace Users.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddDbContext<UserDbContext>(options =>
      options.UseNpgsql(
        configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", UserDbContext.Schema)));

    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IProfileRepository, ProfileRepository>();

    services.AddScoped<IPasswordHasher, PasswordHasher>();
    services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

    return services;
  }
}
