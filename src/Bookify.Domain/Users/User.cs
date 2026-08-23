using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Users;
using Bookify.Domain.Users.Events;

namespace Bookify.Domain.Users;

public class User : Entity
{
    private readonly List<Role> _roles = new();

    private User(Guid id, FirstName firstName, LastName lastName, Email email)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    private User()
    {
    }

    public FirstName FirstName { get; set; }
    public LastName LastName { get; set; }
    public Email Email { get; set; }
    public string IdentityId { get; private set; } = string.Empty;
    public IReadOnlyCollection<Role> Roles => _roles.ToList();

    // Factory method that will allow later to have side-effects that dont belong in constructor
    public static User Create(FirstName firstName, LastName lastName, Email email)
    {
        var user = new User(Guid.NewGuid(), firstName, lastName, email);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

        user._roles.Add(Role.Registered);

        return user;
    }

    public void SetIdentityId(string identityId)
    {
        IdentityId = identityId;
    }
}