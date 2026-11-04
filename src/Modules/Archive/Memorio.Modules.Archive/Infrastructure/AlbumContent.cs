using Dapper;
using Memorio.Modules.Archive.Application.Commands.AddPhoto;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Archive.Domain.Memories;
using Memorio.Modules.Archive.Infrastructure.Persistence;
using Memorio.Modules.Limits.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Archive.Infrastructure;

internal sealed class AlbumContent(ArchiveDbContext database, IMemories memories, ISpacePool personalPool, IAlbumPool albumPool, AddPhotoHandler addPhoto)
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

    public async Task LinkAsync(Guid memoryId, Guid albumId, CancellationToken cancellationToken)
    {
        await database.Database.ExecuteSqlAsync(
            $"INSERT INTO archive.memory_albums (memory_id, album_id) VALUES ({memoryId}, {albumId}) ON CONFLICT DO NOTHING",
            cancellationToken);
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

        foreach (var photo in memory.Photos.Where(photo => photo.AlbumId == albumId))
        {
            await albumPool.ReleaseAsync(photo.Id, cancellationToken);

            // Over the author's limit the photo stays: taking it along must not lose it, and nothing new fits then.
            await personalPool.TryReserveAsync(memory.AuthorAccountId, PoolKeys.For(photo.Id, memory.AuthorAccountId), photo.SizeBytes, cancellationToken);
            photo.LeaveAlbumPool();
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
