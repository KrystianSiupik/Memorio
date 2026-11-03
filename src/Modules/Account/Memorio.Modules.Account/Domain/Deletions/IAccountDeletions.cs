using Memorio.Modules.Account.Domain.Accounts;

namespace Memorio.Modules.Account.Domain.Deletions;

internal interface IAccountDeletions
{
    Task<AccountDeletion?> FindAsync(AccountId accountId, CancellationToken cancellationToken);

    void Add(AccountDeletion deletion);

    /// <summary>
    /// Stores the changes with the events of the aggregate in one transaction. False when another decision
    /// about this deletion got there first: nothing was stored, and <see cref="FindAsync"/> reads the winner.
    /// </summary>
    Task<bool> SaveAsync(AccountDeletion deletion, CancellationToken cancellationToken);
}
