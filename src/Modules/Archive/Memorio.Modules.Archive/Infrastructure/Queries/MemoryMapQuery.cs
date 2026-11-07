using Dapper;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Archive.Application.Queries.MemoryMap;
using Memorio.Modules.Archive.Infrastructure.Persistence;
using Memorio.SharedKernel.Storage;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Archive.Infrastructure.Queries;

/// <summary>The person's own memories and those in albums they take part in now; an album read through a link stays out of it (ADR 0011, D-277).</summary>
internal sealed class MemoryMapQuery(ArchiveDbContext database, IAccessReader access, IObjectStorage storage) : IMemoryMapQuery
{
    private const string FirstPhotosSql = """
        SELECT DISTINCT ON (memory_id) memory_id AS "MemoryId", storage_key AS "StorageKey"
        FROM archive.photos WHERE memory_id = ANY(@ids) ORDER BY memory_id, id
        """;

    private const string Sql = """
        SELECT id AS "Id", latitude AS "Latitude", longitude AS "Longitude", author_account_id = @accountId AS "IsMine"
        FROM archive.memories
        WHERE state = 'Active'
          AND (author_account_id = @accountId
               OR EXISTS (SELECT 1 FROM archive.memory_albums link WHERE link.memory_id = archive.memories.id AND link.album_id = ANY(@visibleAlbums)))
          AND (@from::date IS NULL OR date_to >= @from::date)
          AND (@to::date IS NULL OR date_from <= @to::date)
        """;

    public async Task<IReadOnlyList<MemoryPoint>> GetAsync(Guid accountId, DateOnly? from, DateOnly? to, CancellationToken cancellationToken, bool withPhotos = false)
    {
        var visibleAlbums = (await access.AlbumsTakingPartInAsync(accountId, cancellationToken)).ToArray();
        var points = (await database.Database.GetDbConnection().QueryAsync<PointRow>(new CommandDefinition(
            Sql,
            new { accountId, visibleAlbums, from = from?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture), to = to?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) },
            cancellationToken: cancellationToken))).ToList();
        var hidden = await access.HiddenFromAsync(accountId, [.. points.Where(point => !point.IsMine).Select(point => point.Id)], cancellationToken);
        var shown = points.Where(point => !hidden.Contains(point.Id)).ToList();
        var photos = withPhotos && shown.Count > 0 ? await FirstPhotosAsync([.. shown.Select(point => point.Id)], cancellationToken) : new Dictionary<Guid, Uri>();
        return [.. shown.Select(point => new MemoryPoint(point.Id, point.Latitude, point.Longitude, photos.GetValueOrDefault(point.Id)))];
    }

    private async Task<Dictionary<Guid, Uri>> FirstPhotosAsync(Guid[] ids, CancellationToken cancellationToken)
    {
        var first = (await database.Database.GetDbConnection().QueryAsync<PhotoRow>(
            new CommandDefinition(FirstPhotosSql, new { ids }, cancellationToken: cancellationToken))).ToList();
        if (first.Count == 0)
        {
            return [];
        }

        var signed = await storage.SignReadsAsync([.. first.Select(photo => new StorageKey(photo.StorageKey)).Distinct()], cancellationToken);
        return first.ToDictionary(photo => photo.MemoryId, photo => signed[new StorageKey(photo.StorageKey)].Url);
    }

    private sealed record PhotoRow(Guid MemoryId, string StorageKey);

    private sealed record PointRow(Guid Id, double Latitude, double Longitude, bool IsMine);
}
