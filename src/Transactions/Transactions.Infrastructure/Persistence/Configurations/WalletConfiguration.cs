using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transactions.Domain.Entities;

namespace Transactions.Infrastructure.Persistence.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
  public void Configure(EntityTypeBuilder<Wallet> builder)
  {
    builder.HasKey(w => w.Id);
    builder.Property(w => w.Name).IsRequired().HasMaxLength(256);
    builder.Property(w => w.InitialBalance).HasColumnType("decimal(18,2)");
    builder.Property(w => w.ProfileId).IsRequired();
    builder.HasIndex(w => w.ProfileId);
    builder.HasOne(w => w.Currency).WithMany().HasForeignKey(w => w.CurrencyId);
  }
}
