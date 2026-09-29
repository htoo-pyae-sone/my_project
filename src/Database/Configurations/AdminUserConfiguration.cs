using Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

public class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> b)
    {
        // Keep the storage name explicit so table naming does not depend on EF conventions.
        b.ToTable("admin_users");
        b.HasKey(x => x.Id);

        b.Property(x => x.PublicId).IsRequired();
        b.Property(x => x.UserName).HasMaxLength(50).IsRequired();
        b.Property(x => x.Email).HasMaxLength(254).IsRequired();
        b.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
        b.Property(x => x.IsActive).IsRequired();

        // These values are used to identify accounts, so duplicates must be rejected by the database.
        b.HasIndex(x => x.PublicId).IsUnique();
        b.HasIndex(x => x.UserName).IsUnique();
        b.HasIndex(x => x.Email).IsUnique();

        // Restrict deletion of an admin who is referenced by audit fields; preserve the audit chain.
        b.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.CreatedUsers)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.UpdatedByUser)
            .WithMany(x => x.UpdatedUsers)
            .HasForeignKey(x => x.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
