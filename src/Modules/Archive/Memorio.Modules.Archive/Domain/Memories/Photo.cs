namespace Memorio.Modules.Archive.Domain.Memories;

/// <summary>A photo of a memory; the file itself is in the object storage under <see cref="StorageKey"/>.</summary>
internal sealed class Photo
{
    private Photo()
    {
    }

    public Guid Id { get; private set; }

    public string StorageKey { get; private set; } = "";

    public static Photo Create(string storageKey) => new() { Id = Guid.CreateVersion7(), StorageKey = storageKey };
}
