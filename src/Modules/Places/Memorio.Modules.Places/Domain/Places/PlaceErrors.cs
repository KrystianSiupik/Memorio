using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Places.Domain.Places;

internal static class PlaceErrors
{
    public static readonly Error InvalidName = Error.Validation("Places.InvalidName", $"A place has a name of up to {OwnPlace.MaximumNameLength} characters.");

    public static readonly Error InvalidCoordinates = Error.Validation("Places.InvalidCoordinates", "Latitude is between -90 and 90, longitude between -180 and 180.");

    public static readonly Error InvalidReference = Error.Validation("Places.InvalidReference", "This is not a reference to a place.");

    public static readonly Error NotFound = Error.NotFound("Places.NotFound", "No such place.");

    public static readonly Error IdTaken = Error.Conflict("Places.IdTaken", "This id belongs to another place.");
}
