using Database.Exceptions;
using Shared.Constants;

namespace Database.AppDbContextModels;

/// <summary>Administrative account persisted by the database.</summary>
public class AdminUser : AuditableEntity
{
    /// <summary>Stable externally safe identifier; distinct from the internal database key.</summary>
    public Guid PublicId { get; set; }

    /// <summary>Unique login name.</summary>
    public string UserName { get; private set; } = null!;

    /// <summary>Unique email address associated with the account.</summary>
    public string Email { get; private set; } = null!;

    /// <summary>One-way password hash. Store a verifier output here, never a plaintext password.</summary>
    public string PasswordHash { get; private set; } = null!;

    /// <summary>Whether this account is currently allowed to sign in.</summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>User who created this account, if that actor is an admin user.</summary>
    public AdminUser? CreatedByUser { get; set; }

    /// <summary>Accounts created by this admin user.</summary>
    public ICollection<AdminUser> CreatedUsers { get; set; } = new List<AdminUser>();

    /// <summary>User who last updated this account, if that actor is an admin user.</summary>
    public AdminUser? UpdatedByUser { get; set; }

    /// <summary>Accounts last updated by this admin user.</summary>
    public ICollection<AdminUser> UpdatedUsers { get; set; } = new List<AdminUser>();

    /// <summary>Enables this account to sign in.</summary>
    public void Activate()
    {
        if (IsActive)
            throw new DomainException(ErrorCodes.UserAlreadyActive);

        IsActive = true;
    }

    /// <summary>Disables this account from signing in.</summary>
    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException(ErrorCodes.UserAlreadyInactive);

        IsActive = false;
    }

    /// <summary>Changes the email address after trimming it and checking it is not blank.</summary>
    /// <param name="email">The new email address.</param>
    public void ChangeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException(ErrorCodes.EmailRequired);

        Email = email.Trim();
    }

    /// <summary>Changes the sign-in name after trimming it and checking it is not blank.</summary>
    /// <param name="userName">The new sign-in name.</param>
    public void ChangeUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new DomainException(ErrorCodes.UserNameRequired);

        UserName = userName.Trim();
    }

    /// <summary>Changes the stored password verifier after checking it is not blank.</summary>
    /// <param name="passwordHash">A securely generated password hash, never plaintext.</param>
    public void ChangePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException(ErrorCodes.PasswordHashRequired);

        PasswordHash = passwordHash;
    }
}
