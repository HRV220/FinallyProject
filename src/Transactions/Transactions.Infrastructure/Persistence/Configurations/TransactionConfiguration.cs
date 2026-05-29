using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transactions.Domain.Entities;

namespace Transactions.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
  public void Configure(EntityTypeBuilder<Transaction> builder)
  {
    builder.HasKey(t => t.Id);
    builder.Property(t => t.Amount).HasColumnType("decimal(18,2)");
    builder.Property(t => t.Type).IsRequired();
    builder.Property(t => t.Date).IsRequired();
    builder.Property(t => t.Description).IsRequired(false).HasMaxLength(500);
    builder.Property(t => t.CategoryId).IsRequired(false);
    builder.Property(t => t.ToWalletId).IsRequired(false);
    builder.HasIndex(t => t.WalletId);
    builder.HasIndex(t => t.Date);
    builder.HasOne(t => t.Wallet).WithMany(w => w.Transactions).HasForeignKey(t => t.WalletId).OnDelete(DeleteBehavior.Restrict);
    builder.HasOne(t => t.ToWallet).WithMany().HasForeignKey(t => t.ToWalletId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
  }
}
