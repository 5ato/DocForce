using DocForce.Share;

namespace DocForce.Infrastructure.Common.Outbox;

public class OutboxMessage
{
    private OutboxMessage() { }

    public Guid Id { get; private set; }
    public string EntityType { get; private set; } = string.Empty;
    public string EntityId { get; private set; } = string.Empty;
    public string EventType { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public OutboxMessageStatus Status { get; private set; } = OutboxMessageStatus.Pending;
    public DateTime OccurredAt { get; private set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; private set; }
    public int Retries { get; private set; } = 0;

    public OutboxMessage(string payload, string entityType, string entityId, string eventType)
    {
        Id = Guid.CreateVersion7();

        EntityType = entityType;
        EntityId = entityId;
        EventType = eventType;
        Payload = payload;

        Status = OutboxMessageStatus.Pending;
        OccurredAt = DateTime.UtcNow;
        ProcessedAt = null;
        Retries = 0;
    }

    public void MarkAsFailed()
    {
        Retries++;
        if (Retries >= Constants.MaxOutboxRetries)
        {
            Status = OutboxMessageStatus.Failed;
            ProcessedAt = DateTime.UtcNow;
        }
    }

    public void MarkAsProcessed()
    {
        Status = OutboxMessageStatus.Processed;
        ProcessedAt = DateTime.UtcNow;
    }
}
