using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;
using Bookify.Infrastructure.Clock;
using Bookify.Infrastructure.Data;
using Bookify.Infrastructure.Email;
using Bookify.Infrastructure.Repositories;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        services.AddTransient<IEmailService, EmailService>();

        // Database
        string connectionString = configuration.GetConnectionString("Database") ??
                                  throw new ArgumentNullException(nameof(configuration));
        services.AddDbContext<ApplicationDbContext>(options =>
            // EfCore.NamingConventions snake_case naming columns/tables
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

        // Scoped because for each HTTP req we need to use one DB connection instance
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IApartmentRepository, ApartmentRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        /*
         ReserveBookingCommandHandler
           → depends on IUnitOfWork (defined in Domain)
           → resolved as ApplicationDbContext (Infrastructure)
           → which implements IUnitOfWork via EF Core's DbContext.SaveChangesAsync()
            Here say that UnitOfWork is the ApplicationDbContext
         */
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());


        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(connectionString));
        // Handle DateTime compatibility for Dapper so DB. So DateOnly type works correctly
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());


        return services;
    }
}