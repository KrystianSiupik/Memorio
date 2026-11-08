namespace Memorio.Modules.Album.Contracts;

/// <summary>Read port for AiNarration (R8): the memories an album holds now, with their authors.</summary>
public interface IAlbumMemories
{
    Task<IReadOnlyList<AlbumMemoryRef>> MemoriesOfAsync(Guid albumId, CancellationToken cancellationToken);
}

public sealed record AlbumMemoryRef(Guid MemoryId, Guid AuthorAccountId);
