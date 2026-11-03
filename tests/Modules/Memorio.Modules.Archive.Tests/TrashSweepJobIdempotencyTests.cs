using Memorio.Modules.Archive.Domain.Memories;
using Memorio.Modules.Archive.Infrastructure.Scheduling;
using Memorio.SharedKernel.Scheduling;

namespace Memorio.Modules.Archive.Tests;

/// <summary>SD12, D-228: the daily sweep on a real database. Only backdated windows are overdue, so parallel tests are safe.</summary>
public sealed class TrashSweepJobIdempotencyTests(ArchiveDatabase database)
{
    [Fact]
    public async Task The_same_firing_twice_deletes_an_overdue_memory_once()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        await database.TrashAsync(memory.Id, account);
        await BackdateAsync(memory.Id);
        var firing = Firing();

        await database.FireAsync(firing);
        await database.FireAsync(firing);

        Assert.Null(await database.FindAsync(memory.Id));
    }

    [Fact]
    public async Task A_window_still_open_stays()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        await database.TrashAsync(memory.Id, account);

        await database.FireAsync(Firing());

        Assert.NotNull(await database.FindAsync(memory.Id));
    }

    private static ScheduledJobRun Firing() =>
        new(TrashSweepJob.JobName, null, DateTimeOffset.UtcNow, new Dictionary<string, string>(), Attempt: 1);

    private async Task BackdateAsync(MemoryId id)
    {
        await using var connection = await database.OpenAsAsync(ArchiveDatabase.Role);
        await using var update = new Npgsql.NpgsqlCommand("UPDATE archive.memories SET window_ends_at = now() - interval '1 day' WHERE id = @id", connection);
        update.Parameters.AddWithValue("id", id.Value);
        await update.ExecuteNonQueryAsync(TestContext.Current.CancellationToken);
    }
}
