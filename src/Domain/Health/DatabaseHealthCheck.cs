using Database.AppDbContextModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Domain.Health;

/// <summary>Checks whether the application can connect to its configured database.</summary>
public sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    /// <summary>Creates the check with a scope factory for resolving the database context.</summary>
    /// <param name="scopeFactory">Factory used to resolve the scoped database context.</param>
    /// <param name="logger">Logger used to record database connectivity failures.</param>
    public DatabaseHealthCheck(
        IServiceScopeFactory scopeFactory,
        ILogger<DatabaseHealthCheck> logger
    )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <summary>Attempts to connect to the configured database.</summary>
    /// <param name="context">Health-check metadata supplied by the framework.</param>
    /// <param name="cancellationToken">Token used to cancel the connectivity check.</param>
    /// <returns>A healthy result when the database is reachable; otherwise an unhealthy result.</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? HealthCheckResult.Healthy("Database connection is available.")
                : HealthCheckResult.Unhealthy("Database connection failed.");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Database health check failed");
            return HealthCheckResult.Unhealthy("Database connection failed.", exception);
        }
    }
}
