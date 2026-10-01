namespace Contracts.AdminUserDtos;

/// <summary>Contains required profile fields and an optional account status update.</summary>
public record class UpdateAdminUserDto
{
    /// <summary>Gets or sets the user's sign-in name.</summary>
    public string UserName { get; set; } = null!;

    /// <summary>Gets or sets the user's email address.</summary>
    public string Email { get; set; } = null!;

    /// <summary>Gets or sets the desired account status; null leaves the current status unchanged.</summary>
    public bool? IsActive { get; set; }
}
