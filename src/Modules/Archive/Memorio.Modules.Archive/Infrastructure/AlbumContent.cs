using Dapper;
using Memorio.Modules.Archive.Application.Commands.AddPhoto;
using Memorio.Modules.Archive.Application.Queries.TrimMaterial;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Archive.Domain.Memories;
using Memorio.Modules.Archive.Infrastructure.Persistence;
using Memorio.Modules.Limits.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Archive.Infrastructure;

internal sealed class AlbumContent(ArchiveDbContext database, IMemories memories, ISpacePool personalPool, IAlbumPool albumPool, AddPhotoHandler addPhoto, ITrimMaterialQuery material)
    : IAlbumContent
{
    public async Task<AlbumMemoryFacts?> DescribeAsync(Guid memoryId, CancellationToken cancellationToken)
    {
        var memory = await memories.FindAsync(new MemoryId(memoryId), cancellationToken);
        return memory?.State == MemoryState.Active
            ? new AlbumMemoryFacts(memory.AuthorAccountId, memory.Photos.Sum(photo => photo.SizeBytes))
            : null;
    }

    public async Task<IReadOnlyDictionary<Guid, AlbumMemorySummary>> SummarizeAsync(IReadOnlyCollection<Guid> memoryIds, CancellationToken cancellationToken)
    {
        var rows = await database.Database.GetDbConnection().QueryAsync<SummaryRow>(new CommandDefinition(
            """
            SELECT memory.id AS "Id", memory.place_name AS "PlaceName", memory.date_from::text AS "DateFrom",
                   (SELECT count(*)::int FROM archive.photos photo WHERE photo.memory_id = memory.id) AS "Photos"
            FROM archive.memories memory
            WHERE memory.id = ANY(@memoryIds) AND memory.state = 'Active'
            """,
            new { memoryIds = memoryIds.ToArray() },
            cancellationToken: cancellationToken));
        return rows.ToDictionary(row => row.Id, row => new AlbumMemorySummary(row.PlaceName, row.DateFrom, row.Photos));
    }

    public async Task<IReadOnlyList<Guid>> ChooseToTrimAsync(Guid albumId, IReadOnlyCollection<Guid> keptMemoryIds, long limitBytes, CancellationToken cancellationToken)
    {
        var items = await material.OfAlbumAsync(albumId, cancellationToken);
        return TrimSelection.ToTrim(items.Select(item => item.ToCandidate()), keptMemoryIds, limitBytes);
    }

    public async Task<IReadOnlyList<AlbumTrimMaterial>> TrimMaterialAsync(Guid albumId, CancellationToken cancellationToken) =>
        [.. (await material.OfAlbumAsync(albumId, cancellationToken)).Select(item => new AlbumTrimMaterial(item.MemoryId, item.PlaceName, item.DateFrom, item.SizeBytes))];

    /// <summary>D-264: an album pays for the memory now, so the author's space comes back.</summary>
    public async Task LinkAsync(Guid memoryId, Guid albumId, CancellationToken cancellationToken)
    {
        await database.Database.ExecuteSqlAsync(
            $"INSERT INTO archive.memory_albums (memory_id, album_id) VALUES ({memoryId}, {albumId}) ON CONFLICT DO NOTHING",
            cancellationToken);

        var memory = await memories.FindAsync(new MemoryId(memoryId), cancellationToken);
        foreach (var photo in memory?.Photos.Where(photo => photo.AlbumId is null) ?? [])
        {
            await personalPool.ReleaseAsync(photo.Id, cancellationToken);
        }
    }

    public async Task UnlinkAsync(Guid memoryId, Guid albumId, CancellationToken cancellationToken)
    {
        await database.Database.ExecuteSqlAsync(
            $"DELETE FROM archive.memory_albums WHERE memory_id = {memoryId} AND album_id = {albumId}",
            cancellationToken);

        var memory = await memories.FindAsync(new MemoryId(memoryId), cancellationToken);
        if (memory is null)
        {
            return;
        }

        var inAnotherAlbum = await database.Set<MemoryAlbum>().AnyAsync(link => link.MemoryId == memoryId, cancellationToken);
        foreach (var photo in memory.Photos)
        {
            var paidByThisAlbum = photo.AlbumId == albumId;
            if (paidByThisAlbum)
            {
                await albumPool.ReleaseAsync(photo.Id, cancellationToken);
                photo.LeaveAlbumPool();
            }

            // D-264: while another album holds the memory it pays for the rest; the last one gives everything back.
            if (photo.SizeBytes > 0 && photo.AlbumId is null && (paidByThisAlbum || !inAnotherAlbum))
            {
                await personalPool.ReserveOverLimitAsync(memory.AuthorAccountId, photo.Id, photo.SizeBytes, cancellationToken);
            }
        }

        await memories.SaveAsync(cancellationToken);
    }

    public async Task<AlbumPhotoOutcome> AddPhotoAsync(AlbumPhoto photo, CancellationToken cancellationToken)
    {
        var memory = await memories.FindAsync(new MemoryId(photo.MemoryId), cancellationToken);
        if (memory?.HasPhoto(photo.PhotoId) == true && memory.AuthorAccountId == photo.AccountId)
        {
            return AlbumPhotoOutcome.AlreadyThere;
        }

        var added = await addPhoto.HandleAsync(
            new AddPhoto(new MemoryId(photo.MemoryId), photo.AccountId, photo.PhotoId, photo.UploadKey, photo.AlbumId),
            cancellationToken);
        if (added.IsSuccess)
        {
            return AlbumPhotoOutcome.Added;
        }

        if (added.Error == MemoryErrors.SpaceLimitExceeded)
        {
            return AlbumPhotoOutcome.PoolFull;
        }

        return added.Error == MemoryErrors.UploadMissing || added.Error == MemoryErrors.InvalidUpload
            ? AlbumPhotoOutcome.UploadMissing
            : AlbumPhotoOutcome.NoAccess;
    }

    private sealed record SummaryRow(Guid Id, string PlaceName, string DateFrom, int Photos);
}
