using Memorio.Modules.AiNarration.Domain.Summaries;
using Memorio.Modules.Album.Contracts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.AiNarration.Application.Commands.Summaries;

/// <summary>C84: the person shows their saved summary to the album's participants (HS-009: an entry of the album).</summary>
internal sealed record AddSummaryToAlbum(Guid Id, Guid AccountId);

/// <summary>Only while they still take part in the album; adding it again changes nothing.</summary>
internal sealed class AddSummaryToAlbumHandler(IPeriodSummaries summaries, IAlbumMembership membership, TimeProvider clock)
{
    public async Task<Result<PeriodSummary>> HandleAsync(AddSummaryToAlbum command, CancellationToken cancellationToken)
    {
        if (await summaries.FindAsync(command.Id, cancellationToken) is not { } summary || summary.AccountId != command.AccountId)
        {
            return SummaryErrors.NotFound;
        }

        if (summary.AddedToAlbumAt is null && !await membership.IsMemberAsync(command.AccountId, summary.AlbumId, cancellationToken))
        {
            return SummaryErrors.NotMember;
        }

        var added = summary.AddToAlbum(command.AccountId, clock.GetUtcNow());
        if (added.IsFailure)
        {
            return added.Error;
        }

        if (await summaries.SaveAsync(cancellationToken))
        {
            return summary;
        }

        // Added twice at once, or invalidated meanwhile.
        return await summaries.FindAsync(command.Id, cancellationToken) is { State: SummaryState.Saved, AddedToAlbumAt: not null } raced
            ? raced
            : SummaryErrors.Invalidated;
    }
}
