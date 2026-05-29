using Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Users.Infrastructure.Persistence.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
  public void Configure(EntityTypeBuilder<User> builder)
  {
    builder.HasKey(u => u.Id);
    builder.OwnsOne(u => u.Email, email =>
    {
      email.Property(e => e.Value).HasColumnName("Email");
    });
    builder.OwnsOne(u => u.PasswordHash, hash =>
    {
      hash.Property(h => h.Value).HasColumnName("PasswordHash");
    });
    builder.Property(u => u.FirstName).HasMaxLength(100);
    builder.Property(u => u.LastName).HasMaxLength(100);
    builder.Property(u => u.PhoneNumber).HasMaxLength(20);
    builder.HasMany(u => u.Profiles).WithOne(p => p.User).HasForeignKey(p => p.UserId);
  }
}