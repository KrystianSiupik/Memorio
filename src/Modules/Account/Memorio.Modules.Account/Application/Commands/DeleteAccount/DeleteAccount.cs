using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Domain.Deletions;
using Memorio.Modules.Moderation.Contracts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Account.Application.Commands.DeleteAccount;

/// <summary>
/// C4: the account goes for good, when its window ended. Only the deadline of that window asks for it, and the daily
/// sweep: while an order of an authority holds the account's data, it waits for the next sweep (C114, D-312).
/// </summary>
internal sealed record DeleteAccount(AccountId AccountId, int Sequence, DateTimeOffset At);

internal sealed class DeleteAccountHandler(IAccountDeletions deletions, ILegalHolds holds)
{
    public async Task<Result> HandleAsync(DeleteAccount command, CancellationToken cancellationToken)
    {
        if (await holds.IsHeldAsync(command.AccountId.Value, cancellationToken))
        {
            return AccountErrors.DataHeld;
        }

        var deletion = await deletions.FindAsync(command.AccountId, cancellationToken);
        if (deletion is null)
        {
            return AccountErrors.NoDeletionRequested;
        }

        var completed = deletion.Complete(command.Sequence, command.At);
        if (completed.IsFailure)
        {
            return completed;
        }

        return await deletions.SaveAsync(deletion, cancellationToken) ? Result.Success() : AccountErrors.DeletionAlreadyDecided;
    }
}
