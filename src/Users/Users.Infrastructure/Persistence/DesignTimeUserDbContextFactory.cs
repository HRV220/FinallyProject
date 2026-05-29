using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Users.Infrastructure.Persistence;

public class DesignTimeUserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
  public UserDbContext CreateDbContext(string[] args)
  {
    var apiPath = FindApiProjectPath();

    var configuration = new ConfigurationBuilder()
      .SetBasePath(apiPath)
      .AddJsonFile("appsettings.json", optional: false)
      .AddJsonFile("appsettings.Development.json", optional: true)
      .AddEnvironmentVariables()
      .Build();

    var connectionString = configuration.GetConnectionString("DefaultConnection")
      ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();
    optionsBuilder.UseNpgsql(
      connectionString,
      npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", UserDbContext.Schema));

    return new UserDbContext(optionsBuilder.Options);
  }

  private static string FindApiProjectPath()
  {
    var current = new DirectoryInfo(AppContext.BaseDirectory);
    while (current is not null)
    {
      if (current.GetFiles("*.slnx").Any() || current.GetFiles("*.sln").Any())
      {
        var apiPath = Path.Combine(current.FullName, "src", "Users", "Users.API");
        if (Directory.Exists(apiPath))
          return apiPath;
        throw new InvalidOperationException($"Found solution at '{current.FullName}', but no 'src/Users/Users.API' subfolder.");
      }
      current = current.Parent;
    }
    throw new InvalidOperationException("Could not find solution root.");
  }
}
