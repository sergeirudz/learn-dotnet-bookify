using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Infrastructure.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Bookify.Infrastructure;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    private readonly IDateTimeProvider _dateTimeProvider;


    public ApplicationDbContext(
        DbContextOptions options,
        IDateTimeProvider dateTimeProvider)
        : base(options)
    {
        _dateTimeProvider = dateTimeProvider;
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /*
         * When model is being configured, scans assembly for `IEntityTypeConfiguration`
         * find entity configurations in `src/Bookify.Infrastructure/Configurations`
         * Apply them to EF Data Model
         * Benefit: All files applied automatically
         */
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }


    /*
     * Override native SaveChangesAsync so we can publish Domain Events during operation
     */
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Add events to the change tracker
            AddDomainEventsAsOutboxMessages();

            var result = await base.SaveChangesAsync(cancellationToken);
            // If publish events 
            // then after persisting because Published Event is a FACT that something happened

            return result;
        }
        // Use custom exception so EF details into Application Layer
        catch (DbUpdateConcurrencyException ex)
        {
            // Concurrency occurred in the DB. Pass raw ex data for logging and inspection
            throw new ConcurrencyException("Concurrency exception occurred", ex);
        }
    }

    private void AddDomainEventsAsOutboxMessages()
    {
        // EFCore ChangeTracker grabs Entity entries that implement Entity class 
        var outboxMessages = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            // Map domain events from each Entity 
            .SelectMany(entity =>
            {
                // Save domain events from each entity
                var domainEvents = entity.GetDomainEvents();
                /*
                 * Clear events from Entities
                 * When we publish Domain events, we don't know what can be happening in the handlers
                 */
                entity.ClearDomainEvents();
                // Return Domain events 
                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage(
                Guid.NewGuid(),
                _dateTimeProvider.UtcNow,
                domainEvent.GetType().Name,
                JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings)))
            .ToList();

        AddRange(outboxMessages);
    }
}