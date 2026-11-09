using Memorio.Modules.AiNarration.Domain.Summaries;
using Memorio.Modules.Provenance.Contracts;

namespace Memorio.Modules.AiNarration.Application.Policies;

/// <summary>E61: Provenance found a source of a summary gone; its text goes for good (S2:B2).</summary>
internal sealed class SummaryEndsWithArtifact(IPeriodSummaries summaries)
{
    public async Task HandleAsync(ArtifactInvalidatedEvent invalidated, CancellationToken cancellationToken)
    {
        if (invalidated.Kind != ArtifactKinds.PeriodSummary)
        {
            return;
        }

        // A lost race with an approval is read again: the invalidation must win.
        for (var attempt = 0; attempt < 3; attempt++)
        {
            if (await summaries.FindAsync(invalidated.ArtifactId, cancellationToken) is not { } summary
                || !summary.Invalidate(invalidated.Reason, invalidated.OccurredAt)
                || await summaries.SaveAsync(cancellationToken))
            {
                return;
            }
        }

        throw new InvalidOperationException($"Summary {invalidated.ArtifactId} could not be invalidated.");
    }
}
