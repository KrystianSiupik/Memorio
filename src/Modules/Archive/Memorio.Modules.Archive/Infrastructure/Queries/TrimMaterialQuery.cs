using Dapper;
using Memorio.Modules.Archive.Application.Queries.TrimMaterial;
using Memorio.Modules.Archive.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Archive.Infrastructure.Queries;

internal sealed class TrimMaterialQuery(ArchiveDbContext database) : ITrimMaterialQuery
{
    private const string Columns = """
        SELECT memory.id AS "MemoryId", memory.place_name AS "PlaceName", memory.date_from::text AS "DateFrom",
               COALESCE((SELECT sum(photo.size_bytes) FROM archive.photos photo WHERE photo.memory_id = memory.id), 0)::bigint AS "SizeBytes"
        FROM archive.memories memory
        """;

    private const string AccountSql = Columns + """

        WHERE memory.author_account_id = @id AND memory.state = 'Active'
          AND NOT EXISTS (SELECT 1 FROM archive.memory_albums link WHERE link.memory_id = memory.id)
        ORDER BY memory.date_from, memory.id
        """;

    private const string AlbumSql = Columns + """

        JOIN archive.memory_albums mine ON mine.memory_id = memory.id AND mine.album_id = @id
        WHERE memory.state = 'Active'
        ORDER BY memory.date_from, memory.id
        """;

    public Task<IReadOnlyList<TrimMaterialItem>> OfAccountAsync(Guid accountId, CancellationToken cancellationToken) =>
        ReadAsync(AccountSql, accountId, cancellationToken);

    public Task<IReadOnlyList<TrimMaterialItem>> OfAlbumAsync(Guid albumId, CancellationToken cancellationToken) =>
        ReadAsync(AlbumSql, albumId, cancellationToken);

    private async Task<IReadOnlyList<TrimMaterialItem>> ReadAsync(string sql, Guid id, CancellationToken cancellationToken) =>
        [.. await database.Database.GetDbConnection().QueryAsync<TrimMaterialItem>(new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken))];
}
