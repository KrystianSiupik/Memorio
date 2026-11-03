using Memorio.Modules.Account.Application.Commands.CancelAccountDeletion;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Domain.Deletions;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Account.Tests;

/// <summary>MEM-E1-KONTO-08: E7 in the contract, in the transaction of the change.</summary>
public sealed class AccountDeletedEventTests(AccountHost host)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public void The_event_carries_only_the_effect()
    {
        var names = typeof(AccountDeleted).GetProperties().Select(property => property.Name).Where(name => name != "EqualityContract").Order(StringComparer.Ordinal);

        Assert.Equal(["AccountId", "EventId", "OccurredAt", "Sequence"], names);
    }

    [Fact]
    public async Task The_stored_deletion_reaches_the_worker_with_the_account_and_its_window()
    {
        var account = await host.RegisterAsync();
        var windowEnd = await host.RequestDeletionAsync(account.AccessToken);

        await using (var scope = host.Api.CreateAsyncScope())
        {
            var deletions = scope.ServiceProvider.GetRequiredService<IAccountDeletions>();
            var deletion = (await deletions.FindAsync(new AccountId(account.AccountId), Cancellation))!;
            deletion.Complete(1, windowEnd);
            Assert.True(await deletions.SaveAsync(deletion, Cancellation));
        }

        Assert.True(await AccountHost.EventuallyAsync(() => Task.FromResult(host.Recorded.Deletions(account.AccountId).Count == 1)));
        var deleted = Assert.Single(host.Recorded.Deletions(account.AccountId));
        Assert.Equal(1, deleted.Sequence);
        Assert.Equal(windowEnd, deleted.OccurredAt, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public async Task A_transaction_that_fails_leaves_neither_the_change_nor_the_event()
    {
        var account = await host.RegisterAsync();
        var windowEnd = await host.RequestDeletionAsync(account.AccessToken);
        await using var scope = host.Api.CreateAsyncScope();
        var deletions = scope.ServiceProvider.GetRequiredService<IAccountDeletions>();
        var deletion = (await deletions.FindAsync(new AccountId(account.AccountId), Cancellation))!;

        // The person cancels between loading the window and saving its end: the conditional update fails.
        await using (var other = host.Api.CreateAsyncScope())
        {
            var cancelled = await other.ServiceProvider.GetRequiredService<CancelAccountDeletionHandler>()
                .HandleAsync(new CancelAccountDeletion(new AccountId(account.AccountId)), Cancellation);
            Assert.True(cancelled.IsSuccess);
        }

        deletion.Complete(1, windowEnd);
        Assert.False(await deletions.SaveAsync(deletion, Cancellation));

        await Task.Delay(TimeSpan.FromSeconds(5), Cancellation);
        Assert.Empty(host.Recorded.Deletions(account.AccountId));
        using var me = await host.SendAsync(HttpMethod.Get, "/v1/account/me", account.AccessToken);
        Assert.Equal(System.Net.HttpStatusCode.OK, me.StatusCode);
    }
}
