using Bookify.Api.Extensions;
using Bookify.Application;
using Bookify.Infrastructure;

namespace Bookify.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services.AddApplication(); // Bookify.Application
        builder.Services.AddInfrastructure(builder.Configuration); // Bookify.Infrastructure

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            // app.UseSwagger();
            // app.UseSwaggerUI(options =>
            // {
            //     foreach (ApiVersionDescription description in app.DescribeApiVersions())
            //     {
            //         string url = $"/swagger/{description.GroupName}/swagger.json";
            //         string name = description.GroupName.ToUpperInvariant();
            //         options.SwaggerEndpoint(url, name);
            //     }
            // });

            app.ApplyMigrations();

            // REMARK: Uncomment if you want to seed initial data.
            // app.SeedData();
        }

        app.UseHttpsRedirection();

        app.UseRequestContextLogging();

        // app.UseSerilogRequestLogging();

        app.UseCustomExceptionHandler();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}