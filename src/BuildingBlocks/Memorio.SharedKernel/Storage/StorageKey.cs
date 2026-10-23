namespace Memorio.SharedKernel.Storage;

/// <summary>
/// Path of an object in the private bucket, e.g. "photos/0192.../original.jpg": relative, segments of
/// letters, digits, '-', '_' and '.', never "." or "..". Built by modules, never taken from users.
/// </summary>
public readonly record struct StorageKey
{
    public StorageKey(string value)
    {
        if (!IsValid(value))
        {
            throw new ArgumentException($"Invalid storage key '{value}'.", nameof(value));
        }

        Value = value;
    }

    public string Value { get; }

    /// <summary>Whether the key lies in the temporary upload zone (<see cref="ObjectStorageRules.UploadPrefix"/>).</summary>
    public bool IsUpload => Value.StartsWith(ObjectStorageRules.UploadPrefix, StringComparison.Ordinal);

    public override string ToString() => Value;

    private static bool IsValid(string? value) =>
        !string.IsNullOrEmpty(value)
        && value.Length <= 512
        && value.Split('/').All(segment =>
            segment.Length > 0
            && segment is not "." and not ".."
            && segment.All(character => char.IsAsciiLetterOrDigit(character) || character is '-' or '_' or '.'));
}
