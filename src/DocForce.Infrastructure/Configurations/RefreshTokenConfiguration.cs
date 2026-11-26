using DocForce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocForce.Infrastructure.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.Token)
            .IsRequired();

        builder.Property(c => c.ExpiresAt)
            .IsRequired();

        builder.Property(c => c.IsRevoked)
            .HasDefaultValue(false);

        builder.HasOne(c => c.User)
            .WithMany(c => c.RefreshTokens)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.Token)
            .IsUnique();
    }
}
