using Microsoft.EntityFrameworkCore;
using Reports.Domain.Entities;

namespace Reports.Infrastructure.Persistence;

public class ReportsDbContext : DbContext
{
  public const string Schema = "reports";

  public DbSet<ReportJob> ReportJobs { get; set; } = null!;

  public ReportsDbContext(DbContextOptions<ReportsDbContext> options) : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReportsDbContext).Assembly);
    modelBuilder.HasDefaultSchema(Schema);
    base.OnModelCreating(modelBuilder);
  }
}
