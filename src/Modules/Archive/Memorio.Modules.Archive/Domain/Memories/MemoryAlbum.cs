namespace Memorio.Modules.Archive.Domain.Memories;

/// <summary>A memory is in this album, so people who may see the album may see it. Written by Album through its port.</summary>
internal sealed class MemoryAlbum
{
    public Guid MemoryId { get; private set; }

    public Guid AlbumId { get; private set; }
}
