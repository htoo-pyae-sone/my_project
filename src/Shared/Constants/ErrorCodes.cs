namespace Shared.Constants;

public static class ErrorCodes
{
    /// <summary>Error code returned when an email address is missing.</summary>
    public const string EmailRequired = "EMAIL_REQUIRED";

    /// <summary>Error code returned when a user name is missing.</summary>
    public const string UserNameRequired = "USERNAME_REQUIRED";

    /// <summary>Error code returned when a password hash is missing.</summary>
    public const string PasswordHashRequired = "PASSWORD_HASH_REQUIRED";

    /// <summary>Error code returned when a password is missing.</summary>
    public const string PasswordRequired = "PASSWORD_REQUIRED";

    /// <summary>Error code returned when an account identifier is invalid.</summary>
    public const string InvalidUserId = "INVALID_USER_ID";

    /// <summary>Error code returned when an administrative user cannot be found.</summary>
    public const string AdminUserNotFound = "ADMIN_USER_NOT_FOUND";

    /// <summary>Error code returned when a username is already assigned to another account.</summary>
    public const string UserNameAlreadyExists = "USERNAME_ALREADY_EXISTS";

    /// <summary>Error code returned when an email is already assigned to another account.</summary>
    public const string EmailAlreadyExists = "EMAIL_ALREADY_EXISTS";

    /// <summary>Error code returned when an active user is activated again.</summary>
    public const string UserAlreadyActive = "USER_ALREADY_ACTIVE";

    /// <summary>Error code returned when an inactive user is deactivated again.</summary>
    public const string UserAlreadyInactive = "USER_ALREADY_INACTIVE";
}
