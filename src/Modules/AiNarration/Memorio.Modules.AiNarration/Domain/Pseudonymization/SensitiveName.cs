using Memorio.SharedKernel.Results;

namespace Memorio.Modules.AiNarration.Domain.Pseudonymization;

internal enum SensitiveNameKind
{
    Person,
    Place,
}

/// <summary>A name of a person or a place that must never reach the AI provider (QS8, ADR 0009).</summary>
internal sealed record SensitiveName
{
    private SensitiveName(SensitiveNameKind kind, string value)
    {
        Kind = kind;
        Value = value;
    }

    public SensitiveNameKind Kind { get; }

    public string Value { get; }

    public static Result<SensitiveName> Create(SensitiveNameKind kind, string? value) =>
        string.IsNullOrWhiteSpace(value) || value.Trim().Length < 2
            ? Error.Validation("AiNarration.SensitiveNameTooShort", "A sensitive name needs at least 2 characters.")
            : new SensitiveName(kind, value.Trim());
}
