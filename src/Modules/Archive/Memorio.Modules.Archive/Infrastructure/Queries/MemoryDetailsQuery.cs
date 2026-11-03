using Dapper;
using Memorio.Modules.Archive.Application.Queries.MemoryDetails;
using Memorio.Modules.Archive.Infrastructure.Persistence;
using Memorio.SharedKernel.Storage;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Archive.Infrastructure.Queries;

/// <summary>
/// Only the author's own memory for now; albums and their access rules come with E3 and E4 (ADR 0011).
/// Photos as addresses signed for this read, valid for 5 minutes.
/// </summary>
internal sealed class MemoryDetailsQuery(ArchiveDbContext database, IObjectStorage storage) : IMemoryDetailsQuery
{
    private const string MemorySql = """
        SELECT id AS "Id", place_name AS "PlaceName", latitude AS "Latitude", longitude AS "Longitude", date_precision AS "DatePrecision",
               date_from::text AS "DateFrom", date_to::text AS "DateTo", author_account_id AS "AuthorAccountId", description AS "Description",
               description_ai_model IS NOT NULL AS "DescriptionIsAi"
        FROM archive.memories
        WHERE id = @memoryId AND author_account_id = @accountId AND state = 'Active'
        """;

    private const string PhotosSql = """
        SELECT id AS "Id", storage_key AS "StorageKey" FROM archive.photos WHERE memory_id = @memoryId ORDER BY id
        """;

    public async Task<MemoryDetails?> GetAsync(Guid memoryId, Guid accountId, CancellationToken cancellationToken)
    {
        var connection = database.Database.GetDbConnection();
        var row = await connection.QuerySingleOrDefaultAsync<MemoryRow>(
            new CommandDefinition(MemorySql, new { memoryId, accountId }, cancellationToken: cancellationToken));
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
