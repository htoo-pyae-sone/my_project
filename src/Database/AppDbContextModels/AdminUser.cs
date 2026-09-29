namespace Database.AppDbContextModels;

/// <summary>Administrative account persisted by the database.</summary>
public class AdminUser : AuditableEntity
{
    /// <summary>Stable externally safe identifier; distinct from the internal database key.</summary>
    public Guid PublicId { get; set; }

    /// <summary>Unique login name.</summary>
    public string UserName { get; set; } = null!;

    /// <summary>Unique email address associated with the account.</summary>
    public string Email { get; set; } = null!;

    /// <summary>One-way password hash. Store a verifier output here, never a plaintext password.</summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>Whether this account is currently allowed to sign in.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>User who created this account, if that actor is an admin user.</summary>
    public AdminUser? CreatedByUser { get; set; }

    /// <summary>Accounts created by this admin user.</summary>
    public ICollection<AdminUser> CreatedUsers { get; set; } = new List<AdminUser>();

    /// <summary>User who last updated this account, if that actor is an admin user.</summary>
    public AdminUser? UpdatedByUser { get; set; }

    /// <summary>Accounts last updated by this admin user.</summary>
    public ICollection<AdminUser> UpdatedUsers { get; set; } = new List<AdminUser>();
}
