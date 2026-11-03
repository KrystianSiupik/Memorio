using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Archive.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>SD6, MEM-E1-ARCHIWUM-03: E7 on a real database, delivered twice and out of order.</summary>
public sealed class AccountDeletedHandlerIdempotencyTests(ArchiveDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_event_deletes_the_content_of_that_person_only()
    {
        var account = Guid.CreateVersion7();
        var other = Guid.CreateVersion7();
        await database.AddMemoryAsync(account, photos: 1);
        await database.AddMemoryAsync(other, photos: 1);

        await DeliverAsync(Deleted(account, sequence: 1));

        Assert.Equal(new AccountContentSummary(0, 0), await SummarizeAsync(account));
        Assert.Equal(new AccountContentSummary(1, 1), await SummarizeAsync(other));
    }

    [Fact]
    public async Task The_same_event_twice_leaves_the_same_state()
    {
        var account = Guid.CreateVersion7();
        await database.AddMemoryAsync(account, photos: 1);
        var deleted = Deleted(account, sequence: 1);

        await DeliverAsync(deleted);
        await DeliverAsync(deleted);

        Assert.Equal(new AccountContentSummary(0, 0), await SummarizeAsync(account));
    }

    [Fact]
    public async Task An_event_older_than_the_last_applied_is_skipped()
    {
        var account = Guid.CreateVersion7();
        await DeliverAsync(Deleted(account, sequence: 2));
        await database.AddMemoryAsync(account, photos: 1);

        await DeliverAsync(Deleted(account, sequence: 1));

        Assert.Equal(new AccountContentSummary(1, 1), await SummarizeAsync(account));
    }

    private static AccountDeleted Deleted(Guid account, int sequence) =>
        new(Guid.CreateVersion7(), DateTimeOffset.UtcNow, account, sequence);

    private async Task DeliverAsync(AccountDeleted deleted)
    {
        await using var scope = database.CreateScope();
        await AccountDeletedHandler.Handle(deleted, scope.ServiceProvider, Cancellation);
    }

    private async Task<AccountContentSummary> SummarizeAsync(Guid account)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IAccountContentReader>().SummarizeAsync(account, Cancellation);
    }
}
