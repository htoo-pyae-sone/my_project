namespace Contracts.AdminUserDtos;

/// <summary>Contains the new password for an administrative user.</summary>
public record class ChangePasswordDto
{
    /// <summary>Gets or sets the plaintext password before it is securely hashed.</summary>
    public string NewPassword { get; set; } = null!;
}
