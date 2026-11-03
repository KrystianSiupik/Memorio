using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Domain.Deletions;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Account.Application.Commands.CancelAccountDeletion;

/// <summary>C5: a person keeps their account after all, while the window lasts.</summary>
internal sealed record CancelAccountDeletion(AccountId AccountId);

internal sealed class CancelAccountDeletionHandler(IAccountDeletions deletions, TimeProvider clock)
{
    public async Task<Result> HandleAsync(CancelAccountDeletion command, CancellationToken cancellationToken)
    {
        var deletion = await deletions.FindAsync(command.AccountId, cancellationToken);
        if (deletion is null)
        {
            return AccountErrors.NoDeletionRequested;
        }

        var cancelled = deletion.Cancel(clock.GetUtcNow());
        if (cancelled.IsFailure)
        {
            return cancelled;
        }

        if (await deletions.SaveAsync(deletion, cancellationToken))
        {
            return Result.Success();
        }

        // Cancelled twice at once is one cancellation; anything else means the account already went.
        var winner = await deletions.FindAsync(command.AccountId, cancellationToken);
        return winner?.State == DeletionState.Cancelled ? Result.Success() : AccountErrors.DeletionAlreadyDecided;
    }
}
