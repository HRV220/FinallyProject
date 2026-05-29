using Categories.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Categories.Infrastructure.Persistence;

public class CategoriesDbContext : DbContext
{
  public const string Schema = "categories";

  public DbSet<Category> Categories { get; set; } = null!;

  public CategoriesDbContext(DbContextOptions<CategoriesDbContext> options) : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(CategoriesDbContext).Assembly);
    modelBuilder.HasDefaultSchema(Schema);
    base.OnModelCreating(modelBuilder);
  }
}
