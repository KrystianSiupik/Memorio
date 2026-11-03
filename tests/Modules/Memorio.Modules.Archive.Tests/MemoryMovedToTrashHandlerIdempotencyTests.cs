using Memorio.Modules.Archive.Application.Policies;
using Memorio.Modules.Archive.Contracts;

namespace Memorio.Modules.Archive.Tests;

/// <summary>SD6, MEM-E2-ARCHIWUM-11: E20 delivered twice, and late, on a real database.</summary>
public sealed class MemoryMovedToTrashHandlerIdempotencyTests(ArchiveDatabase database)
{
    [Fact]
    public async Task The_same_event_twice_leaves_one_deadline_at_the_end_of_the_window()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        var trashed = await database.TrashAsync(memory.Id, account);
        var moved = TrashTestSteps.MovedEvent(trashed);

        await database.DeliverAsync(moved);
        await database.DeliverAsync(moved);

        var deadline = database.Deadlines.Find(TrashWindowDeadline.JobName, memory.Id.Value.ToString());
        Assert.Equal(trashed.WindowEndsAt!.Value, deadline?.ScheduledFor ?? default, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public async Task An_event_older_than_the_memory_is_skipped()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        var first = await database.TrashAsync(memory.Id, account);
        var late = TrashTestSteps.MovedEvent(first);
        await database.RestoreAsync(memory.Id, account);

        await database.DeliverAsync(late);

        Assert.Null(database.Deadlines.Find(TrashWindowDeadline.JobName, memory.Id.Value.ToString()));
    }
}
