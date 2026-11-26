using DocForce.Application.Common.Dispatcher.Interfaces;
using DocForce.Domain.Common.Interfaces;
using DocForce.Infrastructure.Context;
using DocForce.Share;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DocForce.Infrastructure.Common.Outbox;

public class PostgresOutboxDispatcher : BackgroundService
{
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);

    private readonly ILogger<PostgresOutboxDispatcher> _log;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IReadOnlyDictionary<string, Type> _domainEventsType;

    public PostgresOutboxDispatcher(
        ILogger<PostgresOutboxDispatcher> log, 
        IServiceScopeFactory scopeFactory)
    {
        _log = log;
        _scopeFactory = scopeFactory;

        _domainEventsType = typeof(IDomainEvent).Assembly
            .GetTypes()
            .Where(type => typeof(IDomainEvent).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
            .ToDictionary(typeKey => typeKey.Name, type => type);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _log.LogInformation("OutboxDispatcher started");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var processed = await ProcessBatchAsync(cancellationToken);

                if (processed == 0)
                    await Task.Delay(PollingInterval, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Outbox batch failed, will retry");
                await Task.Delay(PollingInterval, cancellationToken);
            }
        }
    }

    private async Task<int> ProcessBatchAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var messages = await GetAllPendingMessages(context, cancellationToken);

        if (messages.Count == 0)
        {
            await transaction.RollbackAsync(cancellationToken);
            return 0;
        }

        foreach (var message in messages)
            await HandleMessageAsync(publisher, message, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        _log.LogInformation("Processed outbox batch: {count}", messages.Count);
        return messages.Count;
    }

    private async Task HandleMessageAsync(IPublisher publisher, OutboxMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_domainEventsType.TryGetValue(message.EventType, out var eventType))
            {
                _log.LogError("Unknown event type {EventType} for message {Id}", message.EventType, message.Id);
                message.MarkAsFailed();

                return;
            }

            var domainEvent = JsonSerializer.Deserialize(message.Payload, eventType);

            if (domainEvent == null)
            {
                _log.LogError("Deserialization returned null for message {Id}", message.Id);
                message.MarkAsFailed();

                return;
            }

            await publisher.Publish((IDomainEvent)domainEvent, cancellationToken);
            message.MarkAsProcessed();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to handle outbox message {Id}", message.Id);
            message.MarkAsFailed();
        }
    }

    private async Task<IList<OutboxMessage>> GetAllPendingMessages(ApplicationContext context, CancellationToken cancellationToken = default)
    {
        return await context.OutboxMessages
            .FromSql($"""
                SELECT * FROM "OutboxMessages"
                WHERE "Status" = '{OutboxMessageStatus.Pending.ToString()}'
                ORDER BY "Id"
                LIMIT {Constants.OutboxBatchSize}
                FOR UPDATE SKIP LOCKED
                """)
            .ToListAsync(cancellationToken);
    }
}
