using Memorio.SharedKernel.Domain;

namespace Memorio.Modules.Archive.Domain.Memories;

/// <summary>
/// What a person recorded. Only what deleting an account needs so far: its author and its photos. Place,
/// date and description come with creating a memory (MEM-E2-ARCHIWUM-01) as an expanding migration.
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

    public IReadOnlyCollection<Photo> Photos => _photos.AsReadOnly();

    public static Memory Create(Guid authorAccountId, DateTimeOffset createdAt) =>
        new(new MemoryId(Guid.CreateVersion7()), authorAccountId, createdAt);

    public void AddPhoto(string storageKey) => _photos.Add(Photo.Create(storageKey));

    public void DeleteDefinitively(DateTimeOffset now) =>
        Raise(new ContentDeletedDefinitively(Guid.CreateVersion7(), now, Id, AuthorAccountId));
}
