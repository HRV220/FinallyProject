using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Reports.Infrastructure.Persistence;

public class DesignTimeReportsDbContextFactory : IDesignTimeDbContextFactory<ReportsDbContext>
{
  public ReportsDbContext CreateDbContext(string[] args)
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

    var optionsBuilder = new DbContextOptionsBuilder<ReportsDbContext>();
    optionsBuilder.UseNpgsql(
      connectionString,
      npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", ReportsDbContext.Schema));

    return new ReportsDbContext(optionsBuilder.Options);
  }

  private static string FindApiProjectPath()
  {
    var current = new DirectoryInfo(AppContext.BaseDirectory);
    while (current is not null)
    {
      if (current.GetFiles("*.slnx").Any() || current.GetFiles("*.sln").Any())
      {
        var apiPath = Path.Combine(current.FullName, "src", "Reports", "Reports.API");
        if (Directory.Exists(apiPath))
          return apiPath;
        throw new InvalidOperationException($"Found solution at '{current.FullName}', but no 'src/Reports/Reports.API' subfolder.");
      }
      current = current.Parent;
    }
    throw new InvalidOperationException("Could not find solution root.");
  }
}
