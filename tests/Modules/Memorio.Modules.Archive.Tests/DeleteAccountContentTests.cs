using Memorio.Modules.Archive.Application.Commands.DeleteAccountContent;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Archive.Domain.Memories;
using Memorio.SharedKernel.Domain;
using Memorio.SharedKernel.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>MEM-E1-ARCHIWUM-01 on a real database.</summary>
public sealed class DeleteAccountContentTests(ArchiveDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_memories_and_files_of_the_account_go_and_each_gives_the_event()
    {
        var account = Guid.CreateVersion7();
        var first = await database.AddMemoryAsync(account, photos: 2);
        var second = await database.AddMemoryAsync(account);

        var events = await HandleAsync(account);

        Assert.Equal(new AccountContentSummary(0, 0), await SummarizeAsync(account));
        Assert.All(first.Photos, photo => Assert.Contains(new StorageKey(photo.StorageKey), database.Storage.Deleted));
        var deleted = events.OfType<ContentDeletedDefinitively>().ToList();
        Assert.Equal(new HashSet<MemoryId> { first.Id, second.Id }, deleted.Select(item => item.MemoryId).ToHashSet());
        Assert.All(deleted, item => Assert.Equal(account, item.AuthorAccountId));
    }

    [Fact]
    public async Task What_others_recorded_stays()
    {
        var other = Guid.CreateVersion7();
        var theirs = await database.AddMemoryAsync(other, photos: 1);

        await HandleAsync(Guid.CreateVersion7());

        Assert.Equal(new AccountContentSummary(1, 1), await SummarizeAsync(other));
        Assert.DoesNotContain(new StorageKey(theirs.Photos.Single().StorageKey), database.Storage.Deleted);
    }

    [Fact]
    public async Task Running_it_again_finds_nothing_and_gives_no_second_event()
    {
        var account = Guid.CreateVersion7();
        await database.AddMemoryAsync(account, photos: 1);

        var first = await HandleAsync(account);
        var second = await HandleAsync(account);

        Assert.Single(first.OfType<ContentDeletedDefinitively>());
        Assert.Empty(second);
    }

    [Fact]
    public async Task When_the_storage_does_not_answer_nothing_is_lost_and_the_retry_finishes()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account, photos: 1);
        var key = new StorageKey(memory.Photos.Single().StorageKey);
        database.Storage.Refuse(key);

        await Assert.ThrowsAsync<InvalidOperationException>(() => HandleAsync(account));

        Assert.Equal(new AccountContentSummary(1, 1), await SummarizeAsync(account));
        database.Storage.Accept(key);

        await HandleAsync(account);

        Assert.Equal(new AccountContentSummary(0, 0), await SummarizeAsync(account));
    }

    private async Task<IReadOnlyList<IDomainEvent>> HandleAsync(Guid account)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<DeleteAccountContentHandler>()
            .HandleAsync(new DeleteAccountContent(account), Cancellation);
    }

    private async Task<AccountContentSummary> SummarizeAsync(Guid account)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IAccountContentReader>().SummarizeAsync(account, Cancellation);
    }
}
