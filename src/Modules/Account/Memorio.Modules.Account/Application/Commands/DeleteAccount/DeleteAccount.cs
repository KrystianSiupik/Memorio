using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Domain.Deletions;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Account.Application.Commands.DeleteAccount;

/// <summary>C4: the account goes for good, when its window ended. Only the deadline of that window asks for it.</summary>
internal sealed record DeleteAccount(AccountId AccountId, int Sequence, DateTimeOffset At);

internal sealed class DeleteAccountHandler(IAccountDeletions deletions)
{
    public async Task<Result> HandleAsync(DeleteAccount command, CancellationToken cancellationToken)
    {
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
