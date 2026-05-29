using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Entities;

namespace Users.Infrastructure.Persistence.Configurations;

public class ProfileConfigurations : IEntityTypeConfiguration<Profile>
{
  public void Configure(EntityTypeBuilder<Profile> builder)
  {
    builder.HasKey(p => p.Id);
    builder.Property(p => p.Name).IsRequired().HasMaxLength(256);
  }
}
