using Memorio.Modules.Account.Application.Policies;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Account.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Account.Tests;

/// <summary>SD6, MEM-E1-KONTO-07: E8 delivered twice, on a real database.</summary>
public sealed class AccountDeletionCancelledHandlerIdempotencyTests(AccountHost host)
{
    [Fact]
    public async Task The_same_event_twice_leaves_no_deadline()
    {
        var account = await host.RegisterAsync();
        var windowEnd = await host.RequestDeletionAsync(account.AccessToken);
        Assert.True(await AccountHost.EventuallyAsync(
            () => Task.FromResult(host.Deadlines.Find(AccountDeletionWindowDeadline.JobName, account.AccountId.ToString()) is not null)));
        using (await host.SendAsync(HttpMethod.Delete, "/v1/account/deletion", account.AccessToken))
        {
        }

        var cancelled = new AccountDeletionCancelled(Guid.CreateVersion7(), windowEnd, account.AccountId, 1);
        for (var delivery = 0; delivery < 2; delivery++)
        {
            await using var scope = host.Worker.CreateAsyncScope();
            await AccountDeletionCancelledHandler.Handle(cancelled, scope.ServiceProvider, TestContext.Current.CancellationToken);
        }

        Assert.Null(host.Deadlines.Find(AccountDeletionWindowDeadline.JobName, account.AccountId.ToString()));
    }
}
