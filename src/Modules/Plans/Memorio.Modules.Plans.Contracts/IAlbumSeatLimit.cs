namespace Memorio.Modules.Plans.Contracts;

/// <summary>Read port for Album: what an album's plan allows now, invited people included (S2:B1).</summary>
public interface IAlbumSeatLimit
{
    Task<int> SeatsOfAsync(Guid albumId, CancellationToken cancellationToken);

    /// <summary>The paid plan ended: everyone keeps reading, but nothing that adds to the album works (S2:B1).</summary>
    Task<bool> IsReadOnlyAsync(Guid albumId, CancellationToken cancellationToken);
}
