using Memorio.Modules.Places.Application.Queries.PlaceLookup;
using Memorio.Modules.Places.Domain.Places;
using Memorio.SharedKernel.Domain;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Places.Application.Commands.ChoosePlace;

/// <summary>C10: a person chooses a suggested place. Nothing is stored here; the memory remembers its place.</summary>
internal sealed record ChoosePlace(Guid AccountId, string? PlaceReference);

/// <summary>E10.</summary>
internal sealed record PlaceChosen(Guid EventId, DateTimeOffset OccurredAt, Guid AccountId, FoundPlace Place) : IDomainEvent;

internal sealed class ChoosePlaceHandler(IPlaceLookup places, TimeProvider clock)
{
    public async Task<Result<PlaceChosen>> HandleAsync(ChoosePlace command, CancellationToken cancellationToken)
    {
        if (!PlaceReference.TryParse(command.PlaceReference, out var reference))
        {
            return PlaceErrors.InvalidReference;
        }

        var place = await places.FindAsync(command.AccountId, reference, cancellationToken);
        return place is null
            ? PlaceErrors.NotFound
            : new PlaceChosen(Guid.CreateVersion7(), clock.GetUtcNow(), command.AccountId, place);
    }
}
