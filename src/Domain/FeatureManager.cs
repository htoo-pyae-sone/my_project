using Database.AppDbContextModels;
using Domain.Features.AdminUserFeature;
using Domain.Health;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Base;
using Shared.Constants;

namespace Domain;

/// <summary>Registers domain-layer services, database dependencies, and health checks.</summary>
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
            configuration[ConfigurationKeys.DefaultConnection]
            ?? throw new InvalidOperationException(
                $"Missing configuration: {ConfigurationKeys.DefaultConnection}"
            );

        // Use MySQL and snake_case table/column names to match the database naming convention.
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseMySQL(connectionString).UseSnakeCaseNamingConvention()
        );

        services.AddHttpContextAccessor();
        services.AddScoped<IBaseService, BaseService>();

        services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("database");

        services.AddScoped<IPasswordHasher<AdminUser>, PasswordHasher<AdminUser>>();
        services.AddScoped<IAdminUserService, AdminUserService>();

        return services;
    }
}
