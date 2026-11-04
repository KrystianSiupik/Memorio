using Memorio.Modules.Album.Domain.Albums;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Limits.Contracts;

namespace Memorio.Modules.Album.Application.Commands.Contributions;

/// <summary>
/// C30, the one way an author's contributions leave an album, whoever asks. Archive and the pool first, then
/// the rows: both are safe to repeat, so a failed attempt is simply retried. The caller saves.
/// </summary>
internal sealed class ContributionWithdrawal(IAlbums albums, IAlbumContent content, IAlbumPool pool)
{
    public async Task WithdrawAsync(SharedAlbum album, Guid authorAccountId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var memories = await albums.FindMemoriesOfAuthorAsync(album.Id, authorAccountId, cancellationToken);
        var kept = await albums.FindKeptAsync(album.Id, authorAccountId, cancellationToken);
        var entrustment = await albums.FindEntrustmentAsync(album.Id, authorAccountId, cancellationToken);
        foreach (var memory in memories)
        {
            await content.UnlinkAsync(memory.MemoryId, album.Id.Value, cancellationToken);
            await pool.ReleaseAsync(PoolKeys.For(album.Id.Value, memory.MemoryId), cancellationToken);
            albums.Remove(memory);
        }

        if (kept is not null)
        {
            albums.Remove(kept);
        }

        if (entrustment is not null)
        {
            albums.Remove(entrustment);
        }

        if (memories.Count > 0 || kept is not null)
        {
            album.RecordContributionsWithdrawn(authorAccountId, now);
        }
    }
}
