using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Base;

/// <summary>Provides shared HTTP response mapping for API controllers.</summary>
[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    /// <summary>Maps an operation result to the matching HTTP response.</summary>
    /// <typeparam name="T">Type of data returned by the operation.</typeparam>
    /// <param name="result">The result to convert to an HTTP response.</param>
    protected IActionResult Execute<T>(Result<T> result) =>
        result.Type switch
        {
            ResultType.Success => Ok(result),
            ResultType.Warning => Ok(result),
            ResultType.Error => BadRequest(result),
            ResultType.ValidationError => BadRequest(result),
            ResultType.InvalidData => BadRequest(result),
            ResultType.DuplicateRecord => Conflict(result),
            ResultType.Conflict => Conflict(result),
            ResultType.NotFound => NotFound(result),
            ResultType.BadRequest => BadRequest(result),
            ResultType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, result),
            ResultType.Unauthorized => Unauthorized(result),
            ResultType.SystemError => StatusCode(
                StatusCodes.Status500InternalServerError,
                result
            ),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result)
        };
}
