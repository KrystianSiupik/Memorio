using Dapper;
using Memorio.Modules.Places.Application.Queries.PlaceSuggestions;
using Memorio.Modules.Places.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Places.Infrastructure.Queries;

/// <summary>Own places first, then OpenStreetMap; nothing kept here, so there is nothing to rebuild.</summary>
internal sealed class PlaceSuggestionsQuery(PlacesDbContext database) : IPlaceSuggestionsQuery
{
    private const int MinimumTextLength = 2;
    private const int MaximumLimit = 20;

    private const string Sql = """
        SELECT place_reference AS "PlaceReference", name AS "Name", latitude AS "Latitude", longitude AS "Longitude", is_own AS "IsOwn"
        FROM (
            SELECT 'own:' || id::text AS place_reference, name, latitude, longitude, true AS is_own
            FROM places.own_places
            WHERE owner_account_id = @accountId AND lower(name) LIKE @prefix ESCAPE ''
            UNION ALL
            SELECT 'osm:' || osm_type || ':' || osm_id::text, name, ST_Y(geom), ST_X(geom), false
            FROM places.osm_places
            WHERE lower(name) LIKE @prefix ESCAPE ''
        ) AS suggestions
        ORDER BY is_own DESC, length(name), name
        LIMIT @limit
        """;

    public async Task<IReadOnlyList<PlaceSuggestion>> FindAsync(Guid accountId, string? text, int limit, CancellationToken cancellationToken)
    {
        var typed = text?.Trim();
        if (typed is null || typed.Length < MinimumTextLength)
        {
            return [];
        }

        var prefix = typed.ToLowerInvariant().Replace(@"\", @"\\", StringComparison.Ordinal)
            .Replace("%", @"\%", StringComparison.Ordinal)
            .Replace("_", @"\_", StringComparison.Ordinal) + "%";
        var found = await database.Database.GetDbConnection().QueryAsync<PlaceSuggestion>(new CommandDefinition(
            Sql,
            new { accountId, prefix, limit = Math.Clamp(limit, 1, MaximumLimit) },
            cancellationToken: cancellationToken));
        return [.. found];
    }
}
