using Memorio.Modules.Album.Domain.Albums;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Album.Application.Commands;

/// <summary>
/// A decision about who is in an album, made again when a parallel one got there first: the loser reads the
/// new state, so it is refused only when the new state really refuses it (a seat taken, not a seat contested).
/// </summary>
internal static class AlbumDecision
{
    private const int Attempts = 3;

    public static async Task<Result> UntilStoredAsync(IAlbums albums, Func<Task<Result>> decide, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < Attempts; attempt++)
        {
            var decided = await decide();
            if (decided.IsFailure)
            {
                return decided;
            }

            if (await albums.SaveAsync(cancellationToken))
            {
                return Result.Success();
            }
        }

        return AlbumErrors.AlreadyDecided;
    }
}
