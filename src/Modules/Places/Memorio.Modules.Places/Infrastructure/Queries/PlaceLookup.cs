using Dapper;
using Memorio.Modules.Places.Application.Queries.PlaceLookup;
using Memorio.Modules.Places.Domain.Places;
using Memorio.Modules.Places.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Places.Infrastructure.Queries;

internal sealed class PlaceLookup(PlacesDbContext database) : IPlaceLookup
{
    private const string OwnSql = """
        SELECT 'own:' || id::text AS "PlaceReference", name AS "Name", latitude AS "Latitude", longitude AS "Longitude", true AS "IsOwn"
        FROM places.own_places WHERE id = @id AND owner_account_id = @accountId
        """;

    private const string OsmSql = """
        SELECT 'osm:' || osm_type || ':' || osm_id::text AS "PlaceReference", name AS "Name", ST_Y(geom) AS "Latitude", ST_X(geom) AS "Longitude", false AS "IsOwn"
        FROM places.osm_places WHERE osm_type = @type AND osm_id = @id
        LIMIT 1
        """;

    public Task<FoundPlace?> FindAsync(Guid accountId, PlaceReference reference, CancellationToken cancellationToken)
    {
        var connection = database.Database.GetDbConnection();
        return reference switch
        {
            OwnReference own => connection.QuerySingleOrDefaultAsync<FoundPlace>(
                new CommandDefinition(OwnSql, new { id = own.Id, accountId }, cancellationToken: cancellationToken)),
            OsmReference osm => connection.QuerySingleOrDefaultAsync<FoundPlace>(
                new CommandDefinition(OsmSql, new { type = osm.Type, id = osm.Id }, cancellationToken: cancellationToken)),
            _ => throw new InvalidOperationException($"Unknown place reference {reference.GetType().Name}."),
        };
    }
}
