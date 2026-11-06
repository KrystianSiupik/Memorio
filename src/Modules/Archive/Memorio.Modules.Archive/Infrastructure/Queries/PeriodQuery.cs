using System.Globalization;
using Dapper;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Archive.Application.Queries.Period;
using Memorio.Modules.Archive.Infrastructure.Persistence;
using Memorio.SharedKernel.Storage;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Archive.Infrastructure.Queries;

/// <summary>The same who-sees-what as the map: one's own, and albums one takes part in now (ADR 0011, D-277).</summary>
internal sealed class PeriodQuery(ArchiveDbContext database, IObjectStorage storage, IAccessReader access) : IPeriodQuery
{
    public const int PhotosPerMemory = 3;

    private const string Overlap = """
        state = 'Active' AND date_to >= @from::date AND date_from <= @to::date
        """;

    private const string Order = "ORDER BY date_from, date_to, time_of_day NULLS LAST, created_at, id";

    private const string Sql = $"""
        SELECT id AS "Id", place_name AS "PlaceName", latitude AS "Latitude", longitude AS "Longitude", date_precision AS "DatePrecision",
               date_from::text AS "DateFrom", date_to::text AS "DateTo", to_char(time_of_day, 'HH24:MI') AS "Time", description AS "Description", author_account_id = @accountId AS "IsMine",
               (SELECT count(*)::int FROM archive.photos photo WHERE photo.memory_id = archive.memories.id) AS "Photos"
        FROM archive.memories
        WHERE {Overlap}
          AND (author_account_id = @accountId
               OR EXISTS (SELECT 1 FROM archive.memory_albums link WHERE link.memory_id = archive.memories.id AND link.album_id = ANY(@visibleAlbums)))
        {Order}
        """;

    private const string OwnSql = $"""
        SELECT id FROM archive.memories WHERE author_account_id = @accountId AND {Overlap} {Order}
        """;

    private const string PhotosSql = """
        SELECT memory_id AS "MemoryId", storage_key AS "StorageKey"
        FROM (SELECT memory_id, storage_key, row_number() OVER (PARTITION BY memory_id ORDER BY id) AS rank FROM archive.photos WHERE memory_id = ANY(@ids)) ranked
        WHERE rank <= @limit
        ORDER BY memory_id, rank
        """;

    public async Task<IReadOnlyList<PeriodMemory>> GetAsync(Guid accountId, DateOnly from, DateOnly to, CancellationToken cancellationToken)
    {
        var connection = database.Database.GetDbConnection();
        var visibleAlbums = (await access.AlbumsTakingPartInAsync(accountId, cancellationToken)).ToArray();
        var rows = (await connection.QueryAsync<Row>(new CommandDefinition(
            Sql,
            new { accountId, visibleAlbums, from = Day(from), to = Day(to) },
            cancellationToken: cancellationToken))).ToList();
        var hidden = await access.HiddenFromAsync(accountId, [.. rows.Where(row => !row.IsMine).Select(row => row.Id)], cancellationToken);
        rows.RemoveAll(row => hidden.Contains(row.Id));
        if (rows.Count == 0)
        {
            return [];
        }

        var photos = (await connection.QueryAsync<PhotoRow>(new CommandDefinition(
            PhotosSql,
            new { ids = rows.Select(row => row.Id).ToArray(), limit = PhotosPerMemory },
            cancellationToken: cancellationToken))).ToList();
        var signed = await storage.SignReadsAsync([.. photos.Select(photo => new StorageKey(photo.StorageKey)).Distinct()], cancellationToken);
        var byMemory = photos.ToLookup(photo => photo.MemoryId, photo => signed[new StorageKey(photo.StorageKey)].Url);
        return [.. rows.Select(row => new PeriodMemory(
            row.Id, row.PlaceName, row.Latitude, row.Longitude, row.DatePrecision, row.DateFrom, row.DateTo, row.Time, row.Description, row.IsMine, row.Photos, [.. byMemory[row.Id]]))];
    }

    public async Task<IReadOnlyList<Guid>> OwnAsync(Guid accountId, DateOnly from, DateOnly to, CancellationToken cancellationToken) =>
        [.. await database.Database.GetDbConnection().QueryAsync<Guid>(new CommandDefinition(
            OwnSql,
            new { accountId, from = Day(from), to = Day(to) },
            cancellationToken: cancellationToken))];

    private static string Day(DateOnly day) => day.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    private sealed record Row(Guid Id, string PlaceName, double Latitude, double Longitude, string DatePrecision, string DateFrom, string DateTo, string? Time, string? Description, bool IsMine, int Photos);

    private sealed record PhotoRow(Guid MemoryId, string StorageKey);
}
