using Memorio.Modules.Plans.Contracts;

namespace Memorio.Modules.Plans.Infrastructure;

/// <summary>The album plan's 10 people (D-080) for every album until the plan cycle of E7 tells paid from free (D-245).</summary>
internal sealed class AlbumSeatLimit : IAlbumSeatLimit
{
    public const int Seats = 10;

    public Task<int> SeatsOfAsync(Guid albumId, CancellationToken cancellationToken) => Task.FromResult(Seats);
}
