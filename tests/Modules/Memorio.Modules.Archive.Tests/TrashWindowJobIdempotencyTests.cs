using Memorio.Modules.Archive.Application.Policies;
using Memorio.Modules.Archive.Contracts;

namespace Memorio.Modules.Archive.Tests;

/// <summary>SD12, MEM-E2-ARCHIWUM-11: from the trash to the end of the window, on a real database.</summary>
public sealed class TrashWindowJobIdempotencyTests(ArchiveDatabase database)
{
    [Fact]
    public async Task The_same_firing_twice_deletes_once()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account, photos: 1);
        await database.DeliverAsync(TrashTestSteps.MovedEvent(await database.TrashAsync(memory.Id, account)));
        var firing = database.Deadlines.Find(TrashWindowDeadline.JobName, memory.Id.Value.ToString())!;

        await database.FireAsync(firing);
        await database.FireAsync(firing);

        Assert.Null(await database.FindAsync(memory.Id));
        Assert.True(await ArchiveDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(ContentDeleted), memory.Id.Value) == 1));
        await Task.Delay(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
        Assert.Equal(1, await database.QueuedEventsAsync(nameof(ContentDeleted), memory.Id.Value));
    }

    [Fact]
    public async Task A_firing_of_an_earlier_window_leaves_the_memory_alone()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        await database.DeliverAsync(TrashTestSteps.MovedEvent(await database.TrashAsync(memory.Id, account)));
        var earlier = database.Deadlines.Find(TrashWindowDeadline.JobName, memory.Id.Value.ToString())!;
        await database.RestoreAsync(memory.Id, account);

        await database.FireAsync(earlier);

        Assert.NotNull(await database.FindAsync(memory.Id));
    }
}
