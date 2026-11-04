using Memorio.Modules.Plans.Contracts;

namespace Memorio.Modules.Plans.Infrastructure;

/// <summary>The 5 people of an album without a paid plan (D-261), for every album until the plan cycle of E7 tells paid from free.</summary>
internal sealed class AlbumSeatLimit : IAlbumSeatLimit
{
    public const int FreeSeats = 5;

    public Task<int> SeatsOfAsync(Guid albumId, CancellationToken cancellationToken) => Task.FromResult(FreeSeats);
}
