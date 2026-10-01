namespace Database.Exceptions;

/// <summary>Represents an error caused by a domain rule violation.</summary>
public sealed class DomainException : Exception
{
    /// <summary>Gets the stable code identifying the violated domain rule.</summary>
    public string Code { get; }

    /// <summary>Creates an exception identified by a domain error code.</summary>
    /// <param name="code">Stable code identifying the violated domain rule.</param>
    public DomainException(string code)
        : base(code)
    {
        Code = code;
    }
}
