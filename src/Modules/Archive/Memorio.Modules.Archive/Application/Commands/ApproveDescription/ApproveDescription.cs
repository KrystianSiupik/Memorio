using Memorio.Modules.AiNarration.Contracts;
using Memorio.Modules.Archive.Domain.Memories;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Archive.Application.Commands.ApproveDescription;

/// <summary>C16: the author approves the description. <paramref name="AiDraftId"/> names the AI draft it started from, if any.</summary>
internal sealed record ApproveDescription(MemoryId MemoryId, Guid AccountId, string? Text, Guid? AiDraftId);

internal sealed class ApproveDescriptionHandler(IMemories memories, IAiDraftReader drafts, TimeProvider clock)
{
    public async Task<Result> HandleAsync(ApproveDescription command, CancellationToken cancellationToken)
    {
        var memory = await memories.FindAsync(command.MemoryId, cancellationToken);
        if (memory is null || memory.AuthorAccountId != command.AccountId)
        {
            return MemoryErrors.NoAccess;
        }

        MemoryAiOrigin? origin = null;
        if (command.AiDraftId is { } draftId)
        {
            var draft = await drafts.FindAsync(draftId, command.AccountId, cancellationToken);
            if (draft is null)
            {
                return MemoryErrors.DraftNotFound;
            }

            origin = new MemoryAiOrigin(draft.Model, draft.PromptVersion);
        }

        var approved = memory.ApproveDescription(command.Text, origin, clock.GetUtcNow());
        if (approved.IsFailure)
        {
            return approved;
        }

        return await memories.SaveAsync(cancellationToken) ? Result.Success() : MemoryErrors.AlreadyDecided;
    }
}
