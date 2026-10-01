namespace Contracts;

/// <summary>Represents the outcome of an operation and its optional typed data.</summary>
/// <typeparam name="T">Type of data returned by the operation.</typeparam>
public sealed class Result<T>
{
    private Result(bool isSuccess, ResultType type, T? data, string? code, string? message, string? target)
    {
        IsSuccess = isSuccess;
        Type = type;
        Data = data;
        Code = code;
        Message = message;
        Target = target;
    }

    /// <summary>Gets whether the operation completed successfully.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets whether the operation failed.</summary>
    public bool IsError => !IsSuccess;

    /// <summary>Gets the category of this result.</summary>
    public ResultType Type { get; }

    /// <summary>Gets the stable error code, or <see langword="null"/> for success.</summary>
    public string? Code { get; }

    /// <summary>Gets the explanatory message, if one was provided.</summary>
    public string? Message { get; }

    /// <summary>Gets the optional field or target associated with the result.</summary>
    public string? Target { get; }

    /// <summary>Gets the operation data, when available.</summary>
    public T? Data { get; }

    /// <summary>Creates a successful result containing the operation data.</summary>
    /// <param name="data">Data produced by the operation.</param>
    /// <param name="message">Optional success message.</param>
    public static Result<T> Success(T? data = default, string? message = null) =>
        new(true, ResultType.Success, data, null, message, null);

    /// <summary>Creates a failed result and looks up its message by stable error code.</summary>
    /// <param name="type">The failure category; must not be <see cref="ResultType.Success"/>.</param>
    /// <param name="code">Stable code identifying the failure.</param>
    /// <param name="data">Optional data associated with the failure.</param>
    /// <param name="target">Optional field or target associated with the failure.</param>
    public static Result<T> Failure(
        ResultType type,
        string code,
        T? data = default,
        string? target = null
    )
    {
        if (type == ResultType.Success)
            throw new ArgumentException("A failure result cannot have the Success type.", nameof(type));

        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        return new(false, type, data, code, ErrorMessageCatalog.Get(code), target);
    }
}

/// <summary>Loads English error messages embedded in the contracts assembly.</summary>
internal static class ErrorMessageCatalog
{
    private const string ResourceName = "Contracts.Resources.Messages.en.json";
    private static readonly IReadOnlyDictionary<string, string> Messages = LoadMessages();

    /// <summary>Finds a message for an error code, or returns a generic fallback.</summary>
    /// <param name="code">Stable error code used as the resource key.</param>
    public static string Get(string code) =>
        Messages.TryGetValue(code, out var message) ? message : "An error occurred.";

    private static IReadOnlyDictionary<string, string> LoadMessages()
    {
        using var stream = typeof(ErrorMessageCatalog).Assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException($"Embedded message resource '{ResourceName}' was not found.");

        return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(stream)
            ?? throw new InvalidOperationException($"Embedded message resource '{ResourceName}' is empty or invalid.");
    }
}

/// <summary>Classifies the outcome of an operation.</summary>
public enum ResultType
{
    /// <summary>The operation completed successfully.</summary>
    Success,

    /// <summary>The operation failed for an unspecified reason.</summary>
    Error,

    /// <summary>One or more input values failed validation.</summary>
    ValidationError,

    /// <summary>The operation failed because of an unexpected system error.</summary>
    SystemError,

    /// <summary>The requested resource does not exist.</summary>
    NotFound,

    /// <summary>The operation would create a duplicate record.</summary>
    DuplicateRecord,

    /// <summary>The provided data is invalid.</summary>
    InvalidData,

    /// <summary>The request is malformed or otherwise invalid.</summary>
    BadRequest,

    /// <summary>The operation succeeded with a non-fatal warning.</summary>
    Warning,

    /// <summary>The operation conflicts with the current resource state.</summary>
    Conflict,

    /// <summary>The caller is not allowed to perform the operation.</summary>
    Forbidden,

    /// <summary>The caller must authenticate before performing the operation.</summary>
    Unauthorized
}
