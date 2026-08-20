using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Users;
using Bookify.Domain.Users.Events;

namespace Bookify.Domain.User;

public class User : Entity
{
    /*
     * Hide constructor to private. Allows to add side-effects when Creating user
     */
    private User(Guid id, FirstName firstName, LastName lastName, Email email) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public FirstName FirstName { get; set; }
    public LastName LastName { get; set; }
    public Email Email { get; set; }

    // Factory method that will allow later to have side-effects that dont belong in constructor
    public static User Create(FirstName firstName, LastName lastName, Email email)
    {
        var user = new User(Guid.NewGuid(), firstName, lastName, email);

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));

        return user;
    }
}