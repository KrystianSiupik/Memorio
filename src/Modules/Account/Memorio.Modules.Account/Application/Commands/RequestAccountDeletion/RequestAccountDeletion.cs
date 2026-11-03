using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Domain.Deletions;
using Memorio.Modules.Album.Contracts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Account.Application.Commands.RequestAccountDeletion;

/// <summary>C3: a person asks for their account to be deleted, having seen what goes with it.</summary>
/// <param name="ConsentsToAlbumExpiry">Their albums without a successor expire with the account.</param>
internal sealed record RequestAccountDeletion(AccountId AccountId, bool ConsentsToAlbumExpiry);

/// <summary>
/// Albums without a successor hold the request back (E5) only until the person chooses: a successor or
/// the albums expiring. The choice is always available, so deleting is never blocked (HS-003, D-201).
/// </summary>
internal sealed class RequestAccountDeletionHandler(IAccountDeletions deletions, IAccountAlbumsReader albums, TimeProvider clock)
{
    public async Task<Result<DateTimeOffset>> HandleAsync(RequestAccountDeletion command, CancellationToken cancellationToken)
    {
        var deletion = await deletions.FindAsync(command.AccountId, cancellationToken);
        if (deletion?.State == DeletionState.Pending)
        {
            return deletion.WindowEndsAt;
        }

        if (!command.ConsentsToAlbumExpiry)
        {
            var inAlbums = await albums.SummarizeAsync(command.AccountId.Value, cancellationToken);
            if (inAlbums.OwnAlbumsWithoutSuccessor > 0)
            {
                return AccountErrors.AlbumWithoutSuccessor;
            }
        }

        var now = clock.GetUtcNow();
        if (deletion is null)
        {
            deletion = AccountDeletion.Request(command.AccountId, now, command.ConsentsToAlbumExpiry);
            deletions.Add(deletion);
        }
        else
        {
            var requested = deletion.RequestAgain(now, command.ConsentsToAlbumExpiry);
            if (requested.IsFailure)
            {
                return requested.Error;
            }
        }

        if (await deletions.SaveAsync(deletion, cancellationToken))
        {
            return deletion.WindowEndsAt;
        }

        // The same request twice at once: the first one opened the window.
        var winner = await deletions.FindAsync(command.AccountId, cancellationToken);
        return winner?.State == DeletionState.Pending ? winner.WindowEndsAt : AccountErrors.DeletionAlreadyDecided;
    }
}
