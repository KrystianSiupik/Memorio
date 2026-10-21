namespace Memorio.SharedKernel.Results;

/// <summary>
/// A business failure. <see cref="Code"/> is stable and machine-readable ("Album.NotFound");
/// <see cref="Description"/> is for logs and developers, never shown to users as is.
/// </summary>
public sealed record Error
{
    private Error(string code, string description, ErrorType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        Code = code;
        Description = description;
        Type = type;
    }

    public string Code { get; }

    public string Description { get; }

    public ErrorType Type { get; }

    public static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);

    public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);

    public static Error Conflict(string code, string description) => new(code, description, ErrorType.Conflict);

    public static Error Forbidden(string code, string description) => new(code, description, ErrorType.Forbidden);

    public static Error Failure(string code, string description) => new(code, description, ErrorType.Failure);
}
