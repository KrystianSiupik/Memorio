using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Memorio.Modules.Places.Domain.Places;

/// <summary>Which place: "osm:N:123" (type and id in OpenStreetMap) or "own:&lt;guid&gt;". The foreign ids stop here, as text.</summary>
internal abstract record PlaceReference
{
    public static string Own(Guid id) => $"own:{id}";

    public static string Osm(string type, long id) => string.Create(CultureInfo.InvariantCulture, $"osm:{type}:{id}");

    public static bool TryParse(string? value, [NotNullWhen(true)] out PlaceReference? reference)
    {
        reference = null;
        var parts = value?.Split(':') ?? [];
        if (parts is ["own", var own] && Guid.TryParse(own, out var ownId))
        {
            reference = new OwnReference(ownId);
        }
        else if (parts is ["osm", "N" or "W" or "R", var osm] && long.TryParse(osm, NumberStyles.None, CultureInfo.InvariantCulture, out var osmId))
        {
            reference = new OsmReference(parts[1], osmId);
        }

        return reference is not null;
    }
}

internal sealed record OwnReference(Guid Id) : PlaceReference;

internal sealed record OsmReference(string Type, long Id) : PlaceReference;
