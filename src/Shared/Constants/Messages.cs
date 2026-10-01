namespace Shared.Constants;

public static class Messages
{
    /// <summary>Validation message shown when an email address is missing.</summary>
    public const string EmailRequired = "Email is required.";

    /// <summary>Validation message shown when a user name is missing.</summary>
    public const string UserNameRequired = "Username is required.";

    /// <summary>Validation message shown when a password hash is missing.</summary>
    public const string PasswordHashRequired = "Password hash is required.";

    /// <summary>Error message shown when an active user is activated again.</summary>
    public const string UserAlreadyActive = "User is already active.";

    /// <summary>Error message shown when an inactive user is deactivated again.</summary>
    public const string UserAlreadyInactive = "User is already inactive.";
}
