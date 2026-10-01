using Database.Exceptions;

namespace Api.Middleware;

/// <summary>Converts domain rule exceptions into client-readable error responses.</summary>
public sealed class DomainExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DomainExceptionMiddleware> _logger;

    /// <summary>Creates middleware that looks up messages for domain error codes.</summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    /// <param name="configuration">Configuration containing localized error messages.</param>
    /// <param name="logger">Logger used to record domain rule failures.</param>
    public DomainExceptionMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        ILogger<DomainExceptionMiddleware> logger
    )
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>Runs the request and translates domain exceptions into HTTP 400 responses.</summary>
    /// <param name="context">The current HTTP request and response.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException exception)
        {
            _logger.LogWarning(
                "Domain rule {ErrorCode} rejected request {RequestPath}",
                exception.Code,
                context.Request.Path
            );

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                code = exception.Code,
                message = _configuration[exception.Code] ?? "The request could not be completed."
            });
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception while processing request {RequestPath}",
                context.Request.Path
            );
            throw;
        }
    }
}
