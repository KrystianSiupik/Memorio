using Memorio.Modules.Account.Application.Policies;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Account.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Account.Tests;

/// <summary>SD6, MEM-E1-KONTO-07: E4 delivered twice, and out of order, on a real database.</summary>
public sealed class AccountDeletionRequestedHandlerIdempotencyTests(AccountHost host)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_same_event_twice_leaves_one_deadline_at_the_end_of_the_window()
    {
        var account = await host.RegisterAsync();
        var windowEnd = await host.RequestDeletionAsync(account.AccessToken);
        var requested = new AccountDeletionRequested(Guid.CreateVersion7(), DateTimeOffset.UtcNow, account.AccountId, 1, windowEnd);

        await DeliverAsync(requested);
        await DeliverAsync(requested);

        var deadline = host.Deadlines.Find(AccountDeletionWindowDeadline.JobName, account.AccountId.ToString());
        Assert.Equal(windowEnd, deadline?.ScheduledFor ?? default, TimeSpan.FromMilliseconds(1));
        Assert.Equal("1", deadline?.Data[AccountDeletionWindowDeadline.SequenceKey]);
    }

    [Fact]
    public async Task An_event_about_an_earlier_window_arriving_late_does_not_move_the_deadline()
    {
        var account = await host.RegisterAsync();
        await host.RequestDeletionAsync(account.AccessToken);
        using (await host.SendAsync(HttpMethod.Delete, "/v1/account/deletion", account.AccessToken))
        {
        }

        var windowEnd = await host.RequestDeletionAsync(account.AccessToken);
        await DeliverAsync(new AccountDeletionRequested(Guid.CreateVersion7(), DateTimeOffset.UtcNow, account.AccountId, 2, windowEnd));

        await DeliverAsync(new AccountDeletionRequested(Guid.CreateVersion7(), DateTimeOffset.UtcNow, account.AccountId, 1, windowEnd.AddDays(-5)));
        await DeliverAsync(new AccountDeletionCancelled(Guid.CreateVersion7(), DateTimeOffset.UtcNow, account.AccountId, 1));

        var deadline = host.Deadlines.Find(AccountDeletionWindowDeadline.JobName, account.AccountId.ToString());
        Assert.Equal("2", deadline?.Data[AccountDeletionWindowDeadline.SequenceKey]);
    }

    private async Task DeliverAsync(AccountDeletionRequested requested)
    {
        await using var scope = host.Worker.CreateAsyncScope();
        await AccountDeletionRequestedHandler.Handle(requested, scope.ServiceProvider, Cancellation);
    }

    private async Task DeliverAsync(AccountDeletionCancelled cancelled)
    {
        await using var scope = host.Worker.CreateAsyncScope();
        await AccountDeletionCancelledHandler.Handle(cancelled, scope.ServiceProvider, Cancellation);
    }
}
