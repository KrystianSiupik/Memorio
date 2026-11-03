using Memorio.Modules.Archive.Application.Policies;
using Memorio.Modules.Archive.Contracts;

namespace Memorio.Modules.Archive.Tests;

/// <summary>SD6, MEM-E2-ARCHIWUM-11: E24 delivered twice, and late, on a real database.</summary>
public sealed class MemoryRestoredHandlerIdempotencyTests(ArchiveDatabase database)
{
    [Fact]
    public async Task The_same_event_twice_leaves_no_deadline()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        await database.DeliverAsync(TrashTestSteps.MovedEvent(await database.TrashAsync(memory.Id, account)));
        await database.RestoreAsync(memory.Id, account);
        var restored = new MemoryRestoredEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, memory.Id.Value, 3);

        await database.DeliverAsync(restored);
        await database.DeliverAsync(restored);

        Assert.Null(database.Deadlines.Find(TrashWindowDeadline.JobName, memory.Id.Value.ToString()));
    }

    [Fact]
    public async Task A_late_event_does_not_remove_the_deadline_of_a_newer_window()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        await database.TrashAsync(memory.Id, account);
        await database.RestoreAsync(memory.Id, account);
        var newer = await database.TrashAsync(memory.Id, account);
        await database.DeliverAsync(TrashTestSteps.MovedEvent(newer));

        await database.DeliverAsync(new MemoryRestoredEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, memory.Id.Value, 3));

        Assert.NotNull(database.Deadlines.Find(TrashWindowDeadline.JobName, memory.Id.Value.ToString()));
    }
}
