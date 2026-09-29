using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Database.AppDbContextModels;

/// <summary>
/// EF Core unit of work and model configuration for the application's database.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Discover IEntityTypeConfiguration implementations in this assembly so mappings stay
        // separate from the entity classes.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Apply soft deletion consistently to every auditable entity. Queries hide deleted rows
        // by default; administrative recovery queries can opt out with IgnoreQueryFilters().
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
                continue;

            // Build a filter for the current entity type because each model entity has its own CLR type.
            var param = Expression.Parameter(entityType.ClrType, "e");
            var body = Expression.Not(
                Expression.Property(param, nameof(AuditableEntity.IsDeleted))
            );

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(Expression.Lambda(body, param));
        }
    }
}
