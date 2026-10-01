namespace Shared.Constants;

public static class ErrorCodes
{
    /// <summary>Validation message shown when an email address is missing.</summary>
    public const string EmailRequired = "EMAIL_REQUIRED";

    /// <summary>Validation message shown when a user name is missing.</summary>
    public const string UserNameRequired = "USERNAME_REQUIRED";

    /// <summary>Validation message shown when a password hash is missing.</summary>
    public const string PasswordHashRequired = "PASSWORD_HASH_REQUIRED";

    /// <summary>Error message shown when an active user is activated again.</summary>
    public const string UserAlreadyActive = "USER_ALREADY_ACTIVE";

    /// <summary>Error message shown when an inactive user is deactivated again.</summary>
    public const string UserAlreadyInactive = "USER_ALREADY_INACTIVE";
}
