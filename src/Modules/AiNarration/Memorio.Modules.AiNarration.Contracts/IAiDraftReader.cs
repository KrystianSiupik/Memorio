namespace Memorio.Modules.AiNarration.Contracts;

/// <summary>Read port for Archive: where an approved description came from (SD9). The draft of this person only.</summary>
public interface IAiDraftReader
{
    Task<AiDraftInfo?> FindAsync(Guid draftId, Guid accountId, CancellationToken cancellationToken);
}

public sealed record AiDraftInfo(string Text, string Model, string PromptVersion);
