using DocForce.Domain.Entities;
using DocForce.Share;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocForce.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.PasswordHash)
            .IsRequired();

        builder.Property(c => c.Username)
            .HasMaxLength(Constants.UsernameMaxLength)
            .IsRequired();

        builder.HasIndex(c => c.Username)
            .IsUnique();
    }
}
