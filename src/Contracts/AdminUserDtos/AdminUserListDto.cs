namespace Contracts.AdminUserDtos;

/// <summary>Contains the user details returned to administrative user listings.</summary>
public record class AdminUserListDto
{
    /// <summary>Gets or sets the stable public identifier exposed outside the database.</summary>
    public Guid PublicId { get; set; }

    /// <summary>Gets or sets the user's sign-in name.</summary>
    public string UserName { get; set; } = null!;

    /// <summary>Gets or sets the user's email address.</summary>
    public string Email { get; set; } = null!;

    /// <summary>Gets or sets whether the user account is active.</summary>
    public bool IsActive { get; set; }
}
