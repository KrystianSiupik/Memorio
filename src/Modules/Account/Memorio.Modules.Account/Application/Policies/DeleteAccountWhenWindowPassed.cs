using Memorio.Modules.Account.Application.Commands.DeleteAccount;
using Memorio.Modules.Account.Domain.Accounts;

namespace Memorio.Modules.Account.Application.Policies;

/// <summary>E6: the window with this sequence ended at this time.</summary>
internal sealed record DeletionWindowPassed(AccountId AccountId, int Sequence, DateTimeOffset At);

/// <summary>
/// P1: whenever the window of an account passes, delete the account. A window already decided, or an earlier
/// one than the account has now, is refused by the aggregate and that is the end of it: nothing to retry.
/// </summary>
internal sealed class DeleteAccountWhenWindowPassed(DeleteAccountHandler deleteAccount)
{
    public async Task HandleAsync(DeletionWindowPassed passed, CancellationToken cancellationToken)
    {
        await deleteAccount.HandleAsync(new DeleteAccount(passed.AccountId, passed.Sequence, passed.At), cancellationToken);
    }
}
