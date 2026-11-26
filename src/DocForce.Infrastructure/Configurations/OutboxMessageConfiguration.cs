using DocForce.Infrastructure.Common.Outbox;
using DocForce.Share;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocForce.Infrastructure.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.EntityType)
            .HasMaxLength(Constants.EntityTypeMaxLength)
            .IsRequired();

        builder.Property(c => c.EntityId)
            .HasMaxLength(Constants.EntityIdMaxLength)
            .IsRequired();

        builder.Property(c => c.EventType)
            .HasMaxLength(Constants.EventTypeMaxLength)
            .IsRequired();

        builder.Property(c => c.Payload)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(Constants.StatusMaxLength)
            .IsRequired();

        builder.Property(c => c.OccurredAt)
            .IsRequired();

        builder.Property(c => c.Retries)
            .IsRequired();

        builder.HasIndex(c => new { c.Status, c.Id })
            .HasDatabaseName("idx_outbox_pending")
            .HasFilter("\"Status\" = 'Pending'");
    }
}
