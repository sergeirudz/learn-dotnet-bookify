using Bookify.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
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
}