using Dapper;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Archive.Application.Queries.MemoryDetails;
using Memorio.Modules.Archive.Infrastructure.Persistence;
using Memorio.SharedKernel.Storage;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Archive.Infrastructure.Queries;

/// <summary>
/// The author's own memory, one in an album the person may see now, or a private one of someone who named them as their legacy contact (ADR 0011, S2:C8).
/// Photos as addresses signed for this read, valid for 5 minutes.
/// </summary>
internal sealed class MemoryDetailsQuery(ArchiveDbContext database, IObjectStorage storage, IAccessReader access) : IMemoryDetailsQuery
{
    private const string MemorySql = """
        SELECT id AS "Id", place_name AS "PlaceName", latitude AS "Latitude", longitude AS "Longitude", date_precision AS "DatePrecision",
               date_from::text AS "DateFrom", date_to::text AS "DateTo", author_account_id AS "AuthorAccountId", description AS "Description",
               description_ai_model IS NOT NULL AS "DescriptionIsAi"
        FROM archive.memories
        WHERE id = @memoryId AND state = 'Active'
          AND (author_account_id = @accountId
               OR EXISTS (SELECT 1 FROM archive.memory_albums link WHERE link.memory_id = @memoryId AND link.album_id = ANY(@visibleAlbums))
               OR (author_account_id = ANY(@legacyArchives)
                   AND NOT EXISTS (SELECT 1 FROM archive.memory_albums link WHERE link.memory_id = @memoryId)))
        """;

    private const string PhotosSql = """
        SELECT id AS "Id", storage_key AS "StorageKey" FROM archive.photos WHERE memory_id = @memoryId ORDER BY id
        """;

    public async Task<MemoryDetails?> GetAsync(Guid memoryId, Guid accountId, CancellationToken cancellationToken)
    {
        var connection = database.Database.GetDbConnection();
        var visibleAlbums = (await access.VisibleAlbumsAsync(accountId, cancellationToken)).ToArray();
        var legacyArchives = (await access.LegacyArchivesAsync(accountId, cancellationToken)).ToArray();
        var row = await connection.QuerySingleOrDefaultAsync<MemoryRow>(
            new CommandDefinition(MemorySql, new { memoryId, accountId, visibleAlbums, legacyArchives }, cancellationToken: cancellationToken));
        if (row is null)
        {
            return null;
        }

        var photos = (await connection.QueryAsync<PhotoRow>(new CommandDefinition(PhotosSql, new { memoryId }, cancellationToken: cancellationToken))).ToList();
        var signed = await storage.SignReadsAsync([.. photos.Select(photo => new StorageKey(photo.StorageKey))], cancellationToken);
        return new MemoryDetails(
            row.Id,
            row.PlaceName,
            row.Latitude,
            row.Longitude,
            row.DatePrecision,
            row.DateFrom,
            row.DateTo,
            row.AuthorAccountId,
            row.Description,
            row.DescriptionIsAi,
            [.. photos.Select(photo => new MemoryPhoto(photo.Id, signed[new StorageKey(photo.StorageKey)].Url))]);
    }

    private sealed record MemoryRow(
        Guid Id,
        string PlaceName,
        double Latitude,
        double Longitude,
        string DatePrecision,
        string DateFrom,
        string DateTo,
        Guid AuthorAccountId,
        string? Description,
        bool DescriptionIsAi);

    private sealed record PhotoRow(Guid Id, string StorageKey);
}
