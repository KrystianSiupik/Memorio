using Memorio.Modules.Account.Application.Queries.AccountDeletionImpact;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Archive.Contracts;

namespace Memorio.Modules.Account.Infrastructure.Queries;

/// <summary>Asks the modules that own the data each time; nothing is kept here that could go stale.</summary>
internal sealed class AccountDeletionImpactQuery(IAccountContentReader content, IAccountAlbumsReader albums) : IAccountDeletionImpactQuery
{
    public async Task<AccountDeletionImpact> GetAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        var recorded = await content.SummarizeAsync(accountId.Value, cancellationToken);
        var inAlbums = await albums.SummarizeAsync(accountId.Value, cancellationToken);
        return new AccountDeletionImpact(
            recorded.Memories,
            recorded.Photos,
            inAlbums.OwnAlbums,
            inAlbums.OwnAlbumsWithoutSuccessor,
            inAlbums.OtherPeoplesAlbums);
    }
}
