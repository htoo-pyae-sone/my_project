using Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Domain;

/// <summary>Registers domain-layer services and their database dependencies.</summary>
public static class FeatureManager
{
    /// <summary>
    /// Adds the application's database context using the configured default connection string.
    /// </summary>
    public static IServiceCollection AddDomain(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Fail during startup with a clear configuration error instead of deferring the failure
        // until the first database operation.
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Missing ConnectionStrings:DefaultConnection in configuration."
            );

        // Use MySQL and snake_case table/column names to match the database naming convention.
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseMySQL(connectionString).UseSnakeCaseNamingConvention()
        );

        return services;
    }
}
