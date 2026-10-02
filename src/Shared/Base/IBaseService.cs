namespace Shared.Base;

/// <summary>Exposes request-scoped identity and authorization information to application services.</summary>
public interface IBaseService
{
    /// <summary>Gets the bearer token from the current request, when available.</summary>
    string? AccessToken { get; }

    /// <summary>Gets the authenticated user's internal identifier, when available.</summary>
    long? UserId { get; }

    /// <summary>Gets the authenticated user's name, when available.</summary>
    string? UserName { get; }
}
