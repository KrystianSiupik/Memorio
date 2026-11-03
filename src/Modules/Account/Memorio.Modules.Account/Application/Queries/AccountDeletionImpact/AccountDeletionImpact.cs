using Memorio.Modules.Account.Domain.Accounts;

namespace Memorio.Modules.Account.Application.Queries.AccountDeletionImpact;

/// <summary>R2: in numbers, what goes when this account goes. Always the signed-in person's own account.</summary>
internal sealed record AccountDeletionImpact(
    int Memories,
    int Photos,
    int OwnAlbums,
    int OwnAlbumsWithoutSuccessor,
    int OtherPeoplesAlbums);

internal interface IAccountDeletionImpactQuery
{
    Task<AccountDeletionImpact> GetAsync(AccountId accountId, CancellationToken cancellationToken);
}
