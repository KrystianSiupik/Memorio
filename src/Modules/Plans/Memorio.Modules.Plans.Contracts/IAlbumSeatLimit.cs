namespace Memorio.Modules.Plans.Contracts;

/// <summary>Read port for Album: how many people an album's plan allows, invited ones included (S2:B1).</summary>
public interface IAlbumSeatLimit
{
    Task<int> SeatsOfAsync(Guid albumId, CancellationToken cancellationToken);
}
