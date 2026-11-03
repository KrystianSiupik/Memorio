using Memorio.Modules.Places.Domain.Places;
using Memorio.SharedKernel.Domain;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Places.Application.Commands.AddOwnPlace;

/// <summary>C11: a person names a place of their own. The id comes from the client: sending it again is the same place.</summary>
internal sealed record AddOwnPlace(Guid Id, Guid AccountId, string? Name, double Latitude, double Longitude);

/// <summary>E11.</summary>
internal sealed record OwnPlaceAdded(Guid EventId, DateTimeOffset OccurredAt, OwnPlace Place) : IDomainEvent;

internal sealed class AddOwnPlaceHandler(IOwnPlaces places, TimeProvider clock)
{
    public async Task<Result<OwnPlaceAdded>> HandleAsync(AddOwnPlace command, CancellationToken cancellationToken)
    {
        var now = clock.GetUtcNow();
        var created = OwnPlace.Create(command.Id, command.AccountId, command.Name, command.Latitude, command.Longitude, now);
        if (created.IsFailure)
        {
            return created.Error;
        }

        var place = await places.AddOrFindAsync(created.Value, cancellationToken);
        return place.OwnerAccountId == command.AccountId
            ? new OwnPlaceAdded(Guid.CreateVersion7(), now, place)
            : PlaceErrors.IdTaken;
    }
}
