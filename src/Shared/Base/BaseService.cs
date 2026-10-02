using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Shared.Constants;
using Shared.Extensions;

namespace Shared.Base;

/// <summary>Provides request-scoped identity and authorization header information.</summary>
public class BaseService : IBaseService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>Creates the service using the current HTTP context accessor.</summary>
    /// <param name="httpContextAccessor">Accessor for the current request context.</param>
    public BaseService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;
    private HttpRequest? Request => _httpContextAccessor.HttpContext?.Request;

    /// <summary>Gets the authenticated user's internal identifier, when available.</summary>
    public long? UserId => User?.FindFirst(ConstantClaimCode.UserId)?.Value.ToLong();

    /// <summary>Gets the authenticated user's name, when available.</summary>
    public string? UserName => User?.FindFirst(ConstantClaimCode.UserName)?.Value;

    /// <summary>Gets the bearer token from the current request, when available.</summary>
    public string? AccessToken
    {
        get
        {
            var authHeader = Request?.Headers["Authorization"].FirstOrDefault();
            if (
                !string.IsNullOrWhiteSpace(authHeader)
                && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            )
            {
                return authHeader["Bearer ".Length..].Trim();
            }
            return null;
        }
    }
}
