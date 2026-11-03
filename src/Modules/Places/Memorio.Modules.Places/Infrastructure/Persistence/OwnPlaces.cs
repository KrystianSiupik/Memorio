using Memorio.Modules.Places.Domain.Places;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Memorio.Modules.Places.Infrastructure.Persistence;

internal sealed class OwnPlaces(PlacesDbContext database) : IOwnPlaces
{
    public async Task<OwnPlace> AddOrFindAsync(OwnPlace place, CancellationToken cancellationToken)
    {
        if (await FindAsync(place.Id, cancellationToken) is { } existing)
        {
            return existing;
        }

        database.OwnPlaces.Add(place);
        try
        {
            await database.SaveChangesAsync(cancellationToken);
            return place;
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // The same request twice at once.
            database.ChangeTracker.Clear();
            return await FindAsync(place.Id, cancellationToken) ?? throw new InvalidOperationException("The place that won the race is gone.");
        }
    }

    private Task<OwnPlace?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        database.OwnPlaces.FirstOrDefaultAsync(place => place.Id == id, cancellationToken);
}
