using Memorio.Modules.AiNarration.Contracts;
using Memorio.Modules.AiNarration.Domain.Drafts;

namespace Memorio.Modules.AiNarration.Infrastructure;

internal sealed class AiDraftReader(IMemoryDrafts drafts) : IAiDraftReader
{
    public async Task<AiDraftInfo?> FindAsync(Guid draftId, Guid accountId, CancellationToken cancellationToken)
    {
        var draft = await drafts.FindAsync(draftId, cancellationToken);
        return draft is null || draft.AccountId != accountId ? null : new AiDraftInfo(draft.Text, draft.Model, draft.PromptVersion);
    }
}
