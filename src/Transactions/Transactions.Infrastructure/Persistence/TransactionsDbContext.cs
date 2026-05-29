using Microsoft.EntityFrameworkCore;
using Transactions.Domain.Entities;

namespace Transactions.Infrastructure.Persistence;

public class TransactionsDbContext : DbContext
{
  public const string Schema = "transactions";

  public DbSet<Wallet> Wallets { get; set; } = null!;
  public DbSet<Currency> Currencies { get; set; } = null!;
  public DbSet<Transaction> Transactions { get; set; } = null!;

  public TransactionsDbContext(DbContextOptions<TransactionsDbContext> options) : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(TransactionsDbContext).Assembly);
    modelBuilder.HasDefaultSchema(Schema);
    base.OnModelCreating(modelBuilder);
  }
}
