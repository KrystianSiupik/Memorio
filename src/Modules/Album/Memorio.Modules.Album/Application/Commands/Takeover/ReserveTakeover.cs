using Memorio.Modules.Album.Domain.Albums;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Album.Application.Commands.Takeover;

/// <summary>C155: "Take over" holds the album for 30 minutes, so nobody pays for something someone else is paying for (D-004).</summary>
internal sealed record ReserveTakeover(AlbumId AlbumId, Guid AccountId);

internal sealed class ReserveTakeoverHandler(IAlbums albums, TimeProvider clock)
{
    public Task<Result> HandleAsync(ReserveTakeover command, CancellationToken cancellationToken) =>
        AlbumDecision.UntilStoredAsync(
            albums,
            async () =>
            {
                var album = await albums.FindAsync(command.AlbumId, cancellationToken);
                return album is null ? AlbumErrors.NoAccess : album.ReserveTakeover(command.AccountId, clock.GetUtcNow());
            },
            cancellationToken);
}
