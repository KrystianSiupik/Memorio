using Memorio.Modules.Archive.Application.Commands.DeleteAccountContent;
using Memorio.Modules.Archive.Application.Commands.DeleteMemory;
using Memorio.Modules.Archive.Application.Commands.TrashMemory;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Archive.Domain.Memories;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>MEM-E2-ARCHIWUM-12: E22 in the contract, in the transaction of the change, on every path that deletes (D-208).</summary>
public sealed class ContentDeletedEventTests(ArchiveDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public void The_event_carries_only_the_effect()
    {
        var names = typeof(ContentDeleted).GetProperties().Select(property => property.Name).Where(name => name != "EqualityContract").Order(StringComparer.Ordinal);

        Assert.Equal(["AuthorAccountId", "EventId", "MemoryId", "OccurredAt", "Sequence"], names);
    }

    [Fact]
    public async Task Deleting_after_the_window_stores_the_event()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        var (sequence, windowEnd) = await TrashAsync(memory.Id, account);

        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<DeleteMemoryHandler>().HandleAsync(new DeleteMemory(memory.Id, sequence, windowEnd), Cancellation);
        }

        Assert.True(await ArchiveDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(ContentDeleted), memory.Id.Value) == 1));
    }

    [Fact]
    public async Task Deleting_with_the_account_stores_the_event_too()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);

        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<DeleteAccountContentHandler>().HandleAsync(new DeleteAccountContent(account), Cancellation);
        }

        Assert.True(await ArchiveDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(ContentDeleted), memory.Id.Value) == 1));
    }

    [Fact]
    public async Task A_transaction_that_fails_leaves_neither_the_change_nor_the_event()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        var (sequence, windowEnd) = await TrashAsync(memory.Id, account);
        await using var loser = database.CreateScope();
        var memories = loser.ServiceProvider.GetRequiredService<IMemories>();
        var loaded = (await memories.FindAsync(memory.Id, Cancellation))!;

        // Another save of this memory changes its row first; the concurrency token is what is tested, so it is forced.
        await using (var connection = await database.OpenAsAsync(ArchiveDatabase.Role))
        await using (var bump = new Npgsql.NpgsqlCommand("UPDATE archive.memories SET sequence = sequence + 1 WHERE id = @id", connection))
        {
            bump.Parameters.AddWithValue("id", memory.Id.Value);
            await bump.ExecuteNonQueryAsync(Cancellation);
        }

        await using (var other = database.CreateScope())
        {
            Assert.NotNull(await other.ServiceProvider.GetRequiredService<IMemories>().FindAsync(memory.Id, Cancellation));
        }

        loaded.DeleteDefinitively(windowEnd);
        memories.Remove(loaded);
        Assert.False(await memories.SaveAsync(Cancellation));

        await Task.Delay(TimeSpan.FromSeconds(3), Cancellation);
        Assert.Equal(0, await database.QueuedEventsAsync(nameof(ContentDeleted), memory.Id.Value));
        await using var check = database.CreateScope();
        Assert.NotNull(await check.ServiceProvider.GetRequiredService<IMemories>().FindAsync(memory.Id, Cancellation));
        _ = sequence;
    }

    private async Task<(int Sequence, DateTimeOffset WindowEnd)> TrashAsync(MemoryId memory, Guid account)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<TrashMemoryHandler>().HandleAsync(new TrashMemory(memory, account), Cancellation);
        var stored = (await scope.ServiceProvider.GetRequiredService<IMemories>().FindAsync(memory, Cancellation))!;
        return (stored.Sequence, stored.WindowEndsAt!.Value);
    }
}
