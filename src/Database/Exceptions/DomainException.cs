namespace Database.Exceptions;

/// <summary>Represents an error caused by a domain rule violation.</summary>
public sealed class DomainException : Exception
{
    /// <summary>Creates an exception with a message describing the violated rule.</summary>
    /// <param name="message">Description of the domain rule violation.</param>
    public DomainException(string message)
        : base(message) { }
}
