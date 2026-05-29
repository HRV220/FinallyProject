using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transactions.Domain.Entities;

namespace Transactions.Infrastructure.Persistence.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
  public void Configure(EntityTypeBuilder<Currency> builder)
  {
    builder.HasKey(c => c.Id);
    builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
    builder.Property(c => c.Code).IsRequired().HasMaxLength(3);
    builder.Property(c => c.NumericCode).IsRequired().HasMaxLength(3);
    builder.Property(c => c.Nominal).IsRequired();
    builder.Property(c => c.Rate).HasColumnType("decimal(18,4)");
    builder.Property(c => c.UnitRate).HasColumnType("decimal(18,4)");
    builder.HasIndex(c => c.Code).IsUnique();
  }
}
