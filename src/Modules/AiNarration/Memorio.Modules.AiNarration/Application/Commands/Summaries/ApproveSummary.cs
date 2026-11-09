using Memorio.Modules.AiNarration.Application.Queries.SummaryMaterials;
using Memorio.Modules.AiNarration.Domain.Summaries;
using Memorio.Modules.Provenance.Contracts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.AiNarration.Application.Commands.Summaries;

/// <summary>C83: the person who asked for the summary approves it as it is and keeps it (S2:A2: approval before saving).</summary>
internal sealed record ApproveSummary(Guid Id, Guid AccountId);

/// <summary>
/// The materials are read again: a memory it tells of that may no longer be used means asking anew. Provenance records
/// what it was made from before it is saved, so a saved summary is always one the cascade can find (D-023).
/// </summary>
internal sealed class ApproveSummaryHandler(
    IPeriodSummaries summaries,
    ISummaryMaterialsQuery materials,
    IArtifactRegistry registry,
    TimeProvider clock)
{
    public async Task<Result<PeriodSummary>> HandleAsync(ApproveSummary command, CancellationToken cancellationToken)
    {
        if (await summaries.FindAsync(command.Id, cancellationToken) is not { } summary || summary.AccountId != command.AccountId)
        {
            return SummaryErrors.NotFound;
        }

        if (summary.State == SummaryState.Saved)
        {
            return summary;
        }

        if (summary.State == SummaryState.Invalidated)
        {
            return SummaryErrors.Invalidated;
        }

        var now = await materials.ForAsync(command.AccountId, summary.AlbumId, summary.From, summary.To, cancellationToken);
        var usable = now.Facts.Select(fact => fact.MemoryId).ToHashSet();
        if (summary.Sources.Any(source => !usable.Contains(source.MemoryId)))
        {
            return SummaryErrors.MaterialsChanged;
        }

        var registered = await registry.RegisterAsync(
            new NewArtifact(
                summary.Id,
                ArtifactKinds.PeriodSummary,
                summary.AccountId,
                summary.AlbumId,
                summary.MaterialsReadAt,
                [.. summary.Sources.Select(source => new ArtifactSourceItem(source.MemoryId, source.AuthorAccountId, summary.AlbumId))]),
            cancellationToken);
        if (registered.IsFailure)
        {
            return registered.Error;
        }

        var saved = summary.Save(command.AccountId, clock.GetUtcNow());
        if (saved.IsFailure)
        {
            return saved.Error;
        }

        if (await summaries.SaveAsync(cancellationToken))
        {
            return summary;
        }

        // Approved twice at once, or invalidated meanwhile: what was saved first stands.
        return await summaries.FindAsync(command.Id, cancellationToken) is { State: SummaryState.Saved } raced
            ? raced
            : SummaryErrors.Invalidated;
    }
}
