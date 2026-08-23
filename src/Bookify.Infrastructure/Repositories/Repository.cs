using Bookify.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Repositories;

internal abstract class Repository<T>
    where T : Entity
{
    protected readonly ApplicationDbContext DbContext;

    protected Repository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DbContext
            .Set<T>()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    /*
     * Virtual is because `user._roles.Add(Role.Registered);` EF will add Entity to DbContext
     * EF existing object treats as Entities differently
     * EF will attempt to insert this in the DB, will cause duplicate key exception
     * Allows to override `src/Bookify.Infrastructure/Repositories/UserRepository.cs` Add() method
     * Making Add() virtual allows UserRepository to override the default behavior
     * without the base class knowing about Role at all.
     */
    public virtual void Add(T entity)
    {
        DbContext.Add(entity);
    }
}
