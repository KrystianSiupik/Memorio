using Memorio.Modules.Account.Domain.Accounts;

namespace Memorio.Modules.Account.Application.Queries.PendingAccountDeletion;

internal interface IPendingAccountDeletionQuery
{
    /// <summary>When the open window of this account ends; null when no window is open.</summary>
    Task<DateTimeOffset?> GetWindowEndAsync(AccountId accountId, CancellationToken cancellationToken);
}
