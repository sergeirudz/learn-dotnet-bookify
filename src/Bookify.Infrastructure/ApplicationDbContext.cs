using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    // MediatR publisher for Domain Events
    private readonly IPublisher _publisher;
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
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
        var result = await base.SaveChangesAsync(cancellationToken);
        
        // Publish events after persisting because Published Event is a FACT that something happened
        await PublishDomainEventsAsync();

        return result;
    }

    private async Task PublishDomainEventsAsync()
    {
        // EFCore ChangeTracker grabs Entity entries that implement Entity class 
        var domainEvents = ChangeTracker
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
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            // Publish events that will trigger Domain Event handlers
            await _publisher.Publish(domainEvent);
        }
    }
}