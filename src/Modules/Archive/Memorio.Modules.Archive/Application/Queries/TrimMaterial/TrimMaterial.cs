namespace Memorio.Modules.Archive.Application.Queries.TrimMaterial;

/// <summary>
/// What a trim can take, oldest first. For an account: its author's active memories no album pays for (D-264). For an
/// album: its active memories that are in no other album, because trashing one would take it from there too (D-284).
/// </summary>
internal interface ITrimMaterialQuery
{
    Task<IReadOnlyList<TrimMaterialItem>> OfAccountAsync(Guid accountId, CancellationToken cancellationToken);

    Task<IReadOnlyList<TrimMaterialItem>> OfAlbumAsync(Guid albumId, CancellationToken cancellationToken);
}

internal sealed record TrimMaterialItem(Guid MemoryId, string PlaceName, string DateFrom, long SizeBytes);
