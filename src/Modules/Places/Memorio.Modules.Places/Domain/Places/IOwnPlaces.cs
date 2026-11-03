namespace Memorio.Modules.Places.Domain.Places;

internal interface IOwnPlaces
{
    /// <summary>Stores the place, or finds the one already under its id.</summary>
    Task<OwnPlace> AddOrFindAsync(OwnPlace place, CancellationToken cancellationToken);
}
