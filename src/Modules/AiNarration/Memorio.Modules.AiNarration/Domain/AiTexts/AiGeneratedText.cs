using Memorio.SharedKernel.Results;

namespace Memorio.Modules.AiNarration.Domain.AiTexts;

/// <summary>
/// A text written by the AI. It cannot exist without its provenance: the machine-readable AI marker
/// (AI Act art. 50(2), K9), the model and the prompt version (SD9: every stored AI text has both).
/// </summary>
internal sealed record AiGeneratedText
{
    public const int MaxLength = 20_000;

    private AiGeneratedText(string content, string model, string promptVersion, DateTimeOffset generatedAt)
    {
        Content = content;
        Model = model;
        PromptVersion = promptVersion;
        GeneratedAt = generatedAt;
    }

    public string Content { get; }

    /// <summary>Always true: the marker shown to users and exported with the text.</summary>
    public bool IsAiGenerated { get; } = true;

    public string Model { get; }

    /// <summary>"{prompt id}@{version}", e.g. "memory-description-draft@1".</summary>
    public string PromptVersion { get; }

    public DateTimeOffset GeneratedAt { get; }

    public static Result<AiGeneratedText> Create(string? content, string? model, string? promptVersion, DateTimeOffset generatedAt)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return Error.Validation("AiNarration.EmptyText", "The AI returned no text.");
        }

        if (content.Length > MaxLength)
        {
            return Error.Validation("AiNarration.TextTooLong", $"The AI text is longer than {MaxLength} characters.");
        }

        if (string.IsNullOrWhiteSpace(model) || string.IsNullOrWhiteSpace(promptVersion))
        {
            return Error.Validation("AiNarration.MissingProvenance", "An AI text needs its model and prompt version.");
        }

        return new AiGeneratedText(content.Trim(), model, promptVersion, generatedAt);
    }
}
