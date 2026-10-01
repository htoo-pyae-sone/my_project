namespace Contracts.AdminUserDtos;

/// <summary>Contains the client-supplied fields for creating an administrative user.</summary>
public record class CreateAdminUserDto
{
    /// <summary>Gets or sets the user's sign-in name.</summary>
    public string UserName { get; set; } = null!;

    /// <summary>Gets or sets the user's email address.</summary>
    public string Email { get; set; } = null!;

    /// <summary>Gets or sets the new user's plaintext password before it is securely hashed.</summary>
    public string Password { get; set; } = null!;

    /// <summary>Gets or sets whether the account should be active when created.</summary>
    public bool IsActive { get; set; } = true;
}
