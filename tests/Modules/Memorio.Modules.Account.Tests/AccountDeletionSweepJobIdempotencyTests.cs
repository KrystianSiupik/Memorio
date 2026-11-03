using System.Net;
using Memorio.Modules.Account.Infrastructure.Scheduling;
using Memorio.SharedKernel.Scheduling;

namespace Memorio.Modules.Account.Tests;

/// <summary>SD12, D-215: the daily sweep on a real database. Only backdated windows are overdue, so parallel tests are safe.</summary>
public sealed class AccountDeletionSweepJobIdempotencyTests(AccountHost host)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_same_firing_twice_deletes_an_overdue_account_once()
    {
        var account = await host.RegisterAsync();
        await host.RequestDeletionAsync(account.AccessToken);
        await host.BackdateDeletionWindowAsync(account.AccountId);
        var firing = Firing();

        await FakeDeadlines.FireAsync(host.Worker, firing, Cancellation);
        await FakeDeadlines.FireAsync(host.Worker, firing, Cancellation);

        Assert.Equal(HttpStatusCode.Unauthorized, await MeAsync(account.AccessToken));
        Assert.True(await AccountHost.EventuallyAsync(() => Task.FromResult(host.Recorded.Deletions(account.AccountId).Count == 1)));
        await Task.Delay(TimeSpan.FromSeconds(3), Cancellation);
        Assert.Single(host.Recorded.Deletions(account.AccountId));
    }

    [Fact]
    public async Task A_window_still_open_and_a_cancelled_one_stay()
    {
        var open = await host.RegisterAsync();
        await host.RequestDeletionAsync(open.AccessToken);
        var cancelled = await host.RegisterAsync();
        await host.RequestDeletionAsync(cancelled.AccessToken);
        using (await host.SendAsync(HttpMethod.Delete, "/v1/account/deletion", cancelled.AccessToken))
        {
        }

        await host.BackdateDeletionWindowAsync(cancelled.AccountId);

        await FakeDeadlines.FireAsync(host.Worker, Firing(), Cancellation);

        Assert.Equal(HttpStatusCode.OK, await MeAsync(open.AccessToken));
        Assert.Equal(HttpStatusCode.OK, await MeAsync(cancelled.AccessToken));
    }

    private static ScheduledJobRun Firing() =>
        new(AccountDeletionSweepJob.JobName, null, DateTimeOffset.UtcNow, new Dictionary<string, string>(), Attempt: 1);

    private async Task<HttpStatusCode> MeAsync(string accessToken)
    {
        using var me = await host.SendAsync(HttpMethod.Get, "/v1/account/me", accessToken);
        return me.StatusCode;
    }
}
