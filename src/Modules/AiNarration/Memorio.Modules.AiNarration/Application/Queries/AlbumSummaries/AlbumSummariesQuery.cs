using Memorio.Modules.Access.Contracts;
using Memorio.Modules.AiNarration.Domain.Summaries;
using Memorio.Modules.Album.Contracts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.AiNarration.Application.Queries.AlbumSummaries;

/// <summary>
/// The summaries added to an album, for one of its participants. A participant sees only those whose every memory they
/// may read themselves (Access): a summary does not tell what was hidden from them.
/// </summary>
internal sealed class AlbumSummariesQuery(IPeriodSummaries summaries, IAlbumMembership membership, IReadableContent readable)
{
    public async Task<Result<IReadOnlyList<PeriodSummary>>> ForAsync(Guid accountId, Guid albumId, CancellationToken cancellationToken)
    {
        if (!await membership.IsMemberAsync(accountId, albumId, cancellationToken))
        {
            return SummaryErrors.NotMember;
        }

        var added = await summaries.InAlbumAsync(albumId, cancellationToken);
        var sources = added.SelectMany(summary => summary.Sources.Select(source => source.MemoryId)).Distinct().ToList();
        var mayRead = (await readable.ReadableAsync(accountId, albumId, sources, cancellationToken)).ToHashSet();
        return added.Where(summary => summary.Sources.All(source => mayRead.Contains(source.MemoryId))).ToList();
    }
}
