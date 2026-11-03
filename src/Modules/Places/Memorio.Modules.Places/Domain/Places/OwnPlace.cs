namespace Memorio.Modules.Places.Domain.Places;

/// <summary>A place a person named themselves; kept apart from OpenStreetMap data (decision 34, D-053).</summary>
internal sealed class OwnPlace
{
    public const int MaximumNameLength = 200;

    private OwnPlace(Guid id, Guid ownerAccountId, string name, double latitude, double longitude, DateTimeOffset createdAt)
    {
        Id = id;
        OwnerAccountId = ownerAccountId;
        Name = name;
        Latitude = latitude;
        Longitude = longitude;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid OwnerAccountId { get; private set; }

    public string Name { get; private set; }

    public double Latitude { get; private set; }

    public double Longitude { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static SharedKernel.Results.Result<OwnPlace> Create(Guid id, Guid ownerAccountId, string? name, double latitude, double longitude, DateTimeOffset now)
    {
        var trimmed = name?.Trim();
        if (string.IsNullOrEmpty(trimmed) || trimmed.Length > MaximumNameLength)
        {
            return PlaceErrors.InvalidName;
        }

        if (latitude is < -90 or > 90 || longitude is < -180 or > 180)
        {
            return PlaceErrors.InvalidCoordinates;
        }

        return new OwnPlace(id, ownerAccountId, trimmed, latitude, longitude, now);
    }
}
