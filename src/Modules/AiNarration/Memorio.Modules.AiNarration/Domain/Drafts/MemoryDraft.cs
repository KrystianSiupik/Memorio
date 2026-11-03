using Memorio.Modules.AiNarration.Domain.AiTexts;

namespace Memorio.Modules.AiNarration.Domain.Drafts;

/// <summary>E17: a draft of a description, always with its AI marker data (SD9). Kept so a retry gets the same text.</summary>
internal sealed class MemoryDraft
{
    private MemoryDraft(Guid id, Guid accountId, string text, string model, string promptVersion, DateTimeOffset createdAt)
    {
        Id = id;
        AccountId = accountId;
        Text = text;
        Model = model;
        PromptVersion = promptVersion;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid AccountId { get; private set; }

    public string Text { get; private set; }

    public string Model { get; private set; }

    public string PromptVersion { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static MemoryDraft From(Guid id, Guid accountId, AiGeneratedText text) =>
        new(id, accountId, text.Content, text.Model, text.PromptVersion, text.GeneratedAt);
}
