using Memorio.Modules.Album.Application.Commands.Contributions;
using Memorio.Modules.Album.Domain.Albums;
using Memorio.Modules.Archive.Contracts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Album.Application.Commands.CreateAlbum;

/// <summary>C71: an album of what the person replayed. The id comes from the client, so sending again is the same album.</summary>
internal sealed record CreateAlbumFromPeriod(Guid Id, Guid AccountId, string? Name, DateOnly From, DateOnly To);

/// <param name="Added">The person's own memories of the period now in the album.</param>
/// <param name="NotFitting">What did not fit in the album's pool; it stays on the author's map.</param>
internal sealed record PeriodAlbum(int Added, int NotFitting);

/// <summary>
/// Only the person's own memories go in (D-248): somebody else's stay in their albums. Each goes in the usual way (C47),
/// so a memory already there changes nothing, and what does not fit is counted, not refused as a whole (D-301).
/// </summary>
internal sealed class CreateAlbumFromPeriodHandler(CreateAlbumHandler create, AddMemoryToAlbumHandler add, IAlbumContent content)
{
    public const int MaximumMemories = 1_000;

    public async Task<Result<PeriodAlbum>> HandleAsync(CreateAlbumFromPeriod command, CancellationToken cancellationToken)
    {
        var memories = await content.OwnInPeriodAsync(command.AccountId, command.From, command.To, cancellationToken);
        if (memories.Count > MaximumMemories)
        {
            return AlbumErrors.PeriodTooLong;
        }

        var created = await create.HandleAsync(new CreateAlbum(command.Id, command.AccountId, command.Name), cancellationToken);
        if (created.IsFailure)
        {
            return created.Error;
        }

        var (added, notFitting) = (0, 0);
        foreach (var memoryId in memories)
        {
            var result = await add.HandleAsync(new AddMemoryToAlbum(new AlbumId(command.Id), command.AccountId, memoryId), cancellationToken);
            if (result.IsSuccess)
            {
                added++;
            }
            else if (result.Error == AlbumErrors.PoolFull)
            {
                notFitting++;
            }
            else
            {
                return result.Error;
            }
        }

        return new PeriodAlbum(added, notFitting);
    }
}
