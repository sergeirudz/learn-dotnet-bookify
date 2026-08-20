using MediatR;

namespace Bookify.Domain.Abstractions;

/**
 * MediatR publish-subscribe pattern
 * Will allow to have multiple subscribers to this event
 */
public interface IDomainEvent : INotification
{
}
