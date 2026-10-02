namespace Contracts.AdminUserDtos;

/// <summary>Contains the optional fields that can be changed by a partial user update.</summary>
public record class PatchUpdateAdminUserDto
{
    /// <summary>Gets or sets the replacement username, when it should be changed.</summary>
    public string? UserName { get; set; }

    /// <summary>Gets or sets the replacement email address, when it should be changed.</summary>
    public string? Email { get; set; }

    /// <summary>Gets or sets the replacement active status, when it should be changed.</summary>
    public bool? IsActive { get; set; }
}
