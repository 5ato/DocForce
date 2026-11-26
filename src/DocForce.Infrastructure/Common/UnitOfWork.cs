using DocForce.Application.Interfaces;
using DocForce.Domain.Common;
using DocForce.Infrastructure.Common.Outbox;
using DocForce.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DocForce.Infrastructure.Common;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext _context;
    private readonly ILogger<UnitOfWork> _log;

    public UnitOfWork(ILogger<UnitOfWork> log, ApplicationContext context)
    {
        _log = log;
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _log.LogDebug("Saving entities changes in database");
            var result = await _context.SaveChangesAsync(cancellationToken);
            _log.LogInformation("Saved: {count}", result);
            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _log.LogError(ex, "Concurrency conflict while saving changes");
            throw new Exception("Data has been modified by another user", ex);
        }
        catch (DbUpdateException ex)
        {
            _log.LogError(ex, "Database error while saving changes");
            throw new Exception("An error occurred while saving to the database", ex);
        }
    }

    public async Task SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _log.LogDebug("Saving and publish event");

            var domainEntities = _context.ChangeTracker
                .Entries<BaseEntity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Count != 0)
                .ToList();

            if (domainEntities.Count == 0)
            {
                _log.LogDebug("No domain events to publish");

                await SaveChangesAsync(cancellationToken);

                return;
            }

            foreach (var domainEntity in domainEntities)
            {
                var domainEvents = domainEntity.Entity.DomainEvents;

                _log.LogDebug("Publishing: {count} domain event", domainEvents.Count);

                foreach (var domainEvent in domainEvents)
                {
                    var eventType = domainEvent.GetType().Name;
                    var entityType = domainEntity.Entity.GetType().Name;
                    var entityId = domainEntity.Entity.Id.ToString();
                    var payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType());

                    _log.LogDebug("Start publish event: {eventType}", eventType);

                    var outboxMessage = new OutboxMessage(
                        payload,
                        entityType,
                        entityId,
                        eventType);

                    await _context.OutboxMessages.AddAsync(outboxMessage, cancellationToken);

                    _log.LogInformation("Success publish event: {eventType}", eventType);
                }
            }

            await SaveChangesAsync(cancellationToken);
            domainEntities.ForEach(c => c.Entity.ClearDomainEvents());

            return;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error while saving entities");
            throw;
        }
    }
}
