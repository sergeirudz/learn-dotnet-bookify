using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Users.Events;

public class UserCreatedDomainEvent(Guid UserId) : IDomainEvent
{
}