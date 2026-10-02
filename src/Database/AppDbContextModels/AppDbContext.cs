using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Shared.Base;

namespace Database.AppDbContextModels;

/// <summary>
/// EF Core unit of work and model configuration for the application's database.
/// </summary>
public class AppDbContext : DbContext
{
    private readonly IBaseService? _baseService;

    /// <summary>Creates the database context with the current request identity when available.</summary>
    /// <param name="options">Database context configuration.</param>
    /// <param name="baseService">Request-scoped identity provider used for audit fields.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options, IBaseService? baseService = null)
        : base(options)
    {
        _baseService = baseService;
    }

    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Discover IEntityTypeConfiguration implementations in this assembly so mappings stay
        // separate from the entity classes.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    /// <summary>Persists tracked changes after applying audit metadata.</summary>
    /// <param name="acceptAllChangesOnSuccess">Whether EF Core should accept changes after saving.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        StampAuditFields();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    /// <summary>Persists tracked changes asynchronously after applying audit metadata.</summary>
    /// <param name="acceptAllChangesOnSuccess">Whether EF Core should accept changes after saving.</param>
    /// <param name="cancellationToken">Token used to cancel the database operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default
    )
    {
        StampAuditFields();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void StampAuditFields()
    {
        var now = DateTimeOffset.UtcNow;
        var userId = _baseService?.UserId;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now.DateTime;
                    entry.Entity.UpdatedAt = now.DateTime;
                    if (entry.Entity is AuditableEntity addAudit)
                    {
                        addAudit.CreatedBy = userId;
                    }
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now.DateTime;
                    if (entry.Entity is AuditableEntity modifiedAudit)
                    {
                        modifiedAudit.UpdatedBy = userId;
                    }
                    break;

                case EntityState.Deleted:
                    if (entry.Entity is AuditableEntity deletedAudit)
                    {
                        entry.State = EntityState.Modified;
                        deletedAudit.IsDeleted = true;
                        deletedAudit.UpdatedAt = now.DateTime;
                        deletedAudit.UpdatedBy = userId;
                    }
                    break;
            }
        }
    }
}
