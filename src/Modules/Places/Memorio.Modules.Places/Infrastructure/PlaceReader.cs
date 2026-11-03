using Memorio.Modules.Places.Application.Queries.PlaceLookup;
using Memorio.Modules.Places.Contracts;
using Memorio.Modules.Places.Domain.Places;

namespace Memorio.Modules.Places.Infrastructure;

internal sealed class PlaceReader(IPlaceLookup places) : IPlaceReader
{
    public async Task<PlaceInfo?> FindAsync(Guid accountId, string placeReference, CancellationToken cancellationToken)
    {
        if (!PlaceReference.TryParse(placeReference, out var reference))
        {
            return null;
        }

        var place = await places.FindAsync(accountId, reference, cancellationToken);
        return place is null ? null : new PlaceInfo(place.PlaceReference, place.Name, place.Latitude, place.Longitude);
    }
}
