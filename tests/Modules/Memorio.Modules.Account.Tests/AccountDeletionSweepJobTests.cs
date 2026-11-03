using Memorio.Modules.Account.Application.Commands.DeleteAccount;
using Memorio.Modules.Account.Application.Policies;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.Modules.Account.Domain.Deletions;
using Memorio.Modules.Account.Infrastructure.Scheduling;
using Memorio.SharedKernel.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Memorio.Modules.Account.Tests;

/// <summary>D-215: one account failing must not keep the others from going, nor pass unnoticed.</summary>
public sealed class AccountDeletionSweepJobTests
{
    private static readonly DateTimeOffset Requested = new(2026, 9, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Now = Requested.AddDays(40);

    [Fact]
    public async Task A_failing_account_fails_the_firing_only_after_the_others_went()
    {
        var broken = new AccountId(Guid.CreateVersion7());
        var healthy = AccountDeletion.Request(new AccountId(Guid.CreateVersion7()), Requested, consentsToAlbumExpiry: false);
        var deletions = new OverdueDeletions(broken, healthy);
        await using var services = new ServiceCollection()
            .AddScoped<IAccountDeletions>(_ => deletions)
            .AddScoped<DeleteAccountHandler>()
            .AddScoped<DeleteAccountWhenWindowPassed>()
            .BuildServiceProvider();
        var job = new AccountDeletionSweepJob(deletions, services.GetRequiredService<IServiceScopeFactory>(), NullLogger<AccountDeletionSweepJob>.Instance);
        var firing = new ScheduledJobRun(AccountDeletionSweepJob.JobName, null, Now, new Dictionary<string, string>(), Attempt: 1);

        await Assert.ThrowsAsync<InvalidOperationException>(() => job.RunAsync(firing, TestContext.Current.CancellationToken));

        Assert.Equal(DeletionState.Completed, healthy.State);
    }

    private sealed class OverdueDeletions(AccountId broken, AccountDeletion healthy) : IAccountDeletions
    {
        public Task<IReadOnlyList<OverdueDeletion>> FindOverdueAsync(DateTimeOffset at, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<OverdueDeletion>>([new(broken, 1), new(healthy.Id, healthy.Sequence)]);

        public Task<AccountDeletion?> FindAsync(AccountId accountId, CancellationToken cancellationToken) =>
            accountId == broken
                ? throw new InvalidOperationException("The database does not answer for this account.")
                : Task.FromResult<AccountDeletion?>(healthy);

        public void Add(AccountDeletion deletion) => throw new NotSupportedException();

        public Task<bool> SaveAsync(AccountDeletion deletion, CancellationToken cancellationToken) => Task.FromResult(true);
    }
}
