using System.Net;
using Memorio.Modules.Account.Application.Policies;
using Memorio.SharedKernel.Scheduling;

namespace Memorio.Modules.Account.Tests;

/// <summary>SD12, MEM-E1-KONTO-07: the whole way from a request to the end of the window, on a real database.</summary>
public sealed class AccountDeletionWindowJobIdempotencyTests(AccountHost host)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_same_firing_twice_deletes_the_account_once()
    {
        var account = await host.RegisterAsync();
        await host.RequestDeletionAsync(account.AccessToken);
        var firing = await DeadlineAsync(account.AccountId);

        await FakeDeadlines.FireAsync(host.Worker, firing, Cancellation);
        await FakeDeadlines.FireAsync(host.Worker, firing, Cancellation);

        using var me = await host.SendAsync(HttpMethod.Get, "/v1/account/me", account.AccessToken);
        Assert.Equal(HttpStatusCode.Unauthorized, me.StatusCode);
        Assert.True(await AccountHost.EventuallyAsync(() => Task.FromResult(host.Recorded.Deletions(account.AccountId).Count == 1)));
        await Task.Delay(TimeSpan.FromSeconds(3), Cancellation);
        Assert.Single(host.Recorded.Deletions(account.AccountId));
    }

    [Fact]
    public async Task A_firing_of_an_earlier_window_leaves_the_account_alone()
    {
        var account = await host.RegisterAsync();
        await host.RequestDeletionAsync(account.AccessToken);
        var earlier = await DeadlineAsync(account.AccountId);
        using (await host.SendAsync(HttpMethod.Delete, "/v1/account/deletion", account.AccessToken))
        {
        }

        await host.RequestDeletionAsync(account.AccessToken);

        await FakeDeadlines.FireAsync(host.Worker, earlier, Cancellation);

        using var me = await host.SendAsync(HttpMethod.Get, "/v1/account/me", account.AccessToken);
        Assert.Equal(HttpStatusCode.OK, me.StatusCode);
        Assert.Empty(host.Recorded.Deletions(account.AccountId));
    }

    [Fact]
    public async Task After_cancelling_a_firing_on_its_way_leaves_the_account_alone()
    {
        var account = await host.RegisterAsync();
        await host.RequestDeletionAsync(account.AccessToken);
        var firing = await DeadlineAsync(account.AccountId);
        using (await host.SendAsync(HttpMethod.Delete, "/v1/account/deletion", account.AccessToken))
        {
        }

        await FakeDeadlines.FireAsync(host.Worker, firing, Cancellation);

        using var me = await host.SendAsync(HttpMethod.Get, "/v1/account/me", account.AccessToken);
        Assert.Equal(HttpStatusCode.OK, me.StatusCode);
    }

    private async Task<ScheduledJobRun> DeadlineAsync(Guid accountId)
    {
        Assert.True(await AccountHost.EventuallyAsync(
            () => Task.FromResult(host.Deadlines.Find(AccountDeletionWindowDeadline.JobName, accountId.ToString()) is not null)));
        return host.Deadlines.Find(AccountDeletionWindowDeadline.JobName, accountId.ToString())!;
    }
}
