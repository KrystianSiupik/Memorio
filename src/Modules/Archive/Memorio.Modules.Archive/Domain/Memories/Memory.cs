using Memorio.SharedKernel.Domain;

namespace Memorio.Modules.Archive.Domain.Memories;

/// <summary>
/// What a person recorded: where, when and what. The place is a snapshot taken when choosing it, so the
/// map needs no other module. One author for now; contributions of others come with albums (E3, HS-019).
/// </summary>
internal sealed class Memory : AggregateRoot<MemoryId>
{
    private readonly List<Photo> _photos = [];

    private Memory(MemoryId id, Guid authorAccountId, DateTimeOffset createdAt)
        : base(id)
    {
        AuthorAccountId = authorAccountId;
        CreatedAt = createdAt;
    }

    public Guid AuthorAccountId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public string PlaceReference { get; private set; } = "";

    public string PlaceName { get; private set; } = "";

    public double Latitude { get; private set; }

    public double Longitude { get; private set; }

    public DatePrecision DatePrecision { get; private set; }

    public DateOnly DateFrom { get; private set; }

    public DateOnly DateTo { get; private set; }

    public IReadOnlyCollection<Photo> Photos => _photos.AsReadOnly();

    public static Memory Create(MemoryId id, Guid authorAccountId, MemoryPlace place, MemoryDate date, DateTimeOffset now)
    {
        var memory = new Memory(id, authorAccountId, now)
        {
            PlaceReference = place.Reference,
            PlaceName = place.Name,
            Latitude = place.Latitude,
            Longitude = place.Longitude,
            DatePrecision = date.Precision,
            DateFrom = date.From,
            DateTo = date.To,
        };
        memory.Raise(new MemoryCreated(Guid.CreateVersion7(), now, id, authorAccountId));
        return memory;
    }

    public void AddPhoto(string storageKey) => _photos.Add(Photo.Create(storageKey));

    public void DeleteDefinitively(DateTimeOffset now) =>
        Raise(new ContentDeletedDefinitively(Guid.CreateVersion7(), now, Id, AuthorAccountId));
}

/// <summary>What a memory keeps of its place.</summary>
internal sealed record MemoryPlace(string Reference, string Name, double Latitude, double Longitude);
