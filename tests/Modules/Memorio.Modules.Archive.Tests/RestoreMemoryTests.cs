using Memorio.Modules.Archive.Application.Commands.DeleteMemory;
using Memorio.Modules.Archive.Application.Commands.RestoreMemory;
using Memorio.Modules.Archive.Application.Commands.TrashMemory;
using Memorio.Modules.Archive.Application.Queries.MemoryDetails;
using Memorio.Modules.Archive.Domain.Memories;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>MEM-E2-ARCHIWUM-06: AG10 without a database, then on a real one.</summary>
public sealed class RestoreMemoryTests(ArchiveDatabase database)
{
    private static readonly DateTimeOffset Now = new(2026, 11, 3, 12, 0, 0, TimeSpan.Zero);

    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public void During_the_window_it_comes_back_with_a_new_sequence_and_the_event()
    {
        var memory = NewMemory();
        memory.MoveToTrash(Now);
        var sequence = memory.Sequence;
        memory.ClearDomainEvents();

        var restored = memory.Restore(Now.AddDays(29), planPaid: false);

        Assert.True(restored.IsSuccess);
        Assert.Equal(MemoryState.Active, memory.State);
        Assert.Null(memory.WindowEndsAt);
        Assert.Equal(sequence + 1, Assert.IsType<MemoryRestored>(Assert.Single(memory.DomainEvents)).Sequence);
    }

    [Fact]
    public void At_the_end_of_the_window_it_is_too_late()
    {
        var memory = NewMemory();
        memory.MoveToTrash(Now);

        Assert.Equal(MemoryErrors.AlreadyDecided, memory.Restore(Now.AddDays(30), planPaid: false).Error);
        Assert.Equal(MemoryState.Trashed, memory.State);
    }

    [Fact]
    public void What_is_not_in_the_trash_does_not_come_back()
    {
        Assert.Equal(MemoryErrors.AlreadyDecided, NewMemory().Restore(Now, planPaid: false).Error);
    }

    [Fact]
    public void Trimmed_material_comes_back_only_with_a_paid_plan_and_only_during_the_window()
    {
        var memory = NewMemory();
        memory.MoveToTrashByTrimming(Now, Guid.CreateVersion7());

        Assert.Equal(MemoryErrors.PlanRequired, memory.Restore(Now.AddDays(1), planPaid: false).Error);
        Assert.Equal(MemoryErrors.AlreadyDecided, memory.Restore(Now.AddDays(30), planPaid: true).Error);
        Assert.True(memory.Restore(Now.AddDays(1), planPaid: true).IsSuccess);
    }

    [Fact]
    public async Task It_is_back_in_the_details()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        await TrashAsync(memory.Id, account);

        var restored = await RestoreAsync(memory.Id, account);

        Assert.True(restored.IsSuccess);
        await using var scope = database.CreateScope();
        Assert.NotNull(await scope.ServiceProvider.GetRequiredService<IMemoryDetailsQuery>().GetAsync(memory.Id.Value, account, Cancellation));
    }

    [Fact]
    public async Task Someone_elses_memory_is_refused()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        await TrashAsync(memory.Id, account);

        Assert.Equal(MemoryErrors.NoAccess, (await RestoreAsync(memory.Id, Guid.CreateVersion7())).Error);
    }

    [Fact]
    public async Task Restoring_and_deleting_in_parallel_have_one_winner()
    {
        for (var round = 0; round < 10; round++)
        {
            var account = Guid.CreateVersion7();
            var memory = await database.AddMemoryAsync(account);
            var (sequence, windowEnd) = await TrashAsync(memory.Id, account);

            var restoring = RestoreAsync(memory.Id, account);
            var deleting = DeleteAsync(memory.Id, sequence, windowEnd);
            var results = await Task.WhenAll(restoring, deleting);

            Assert.Single(results, result => result.IsSuccess);
            Assert.Equal(MemoryErrors.AlreadyDecided, Assert.Single(results, result => result.IsFailure).Error);
        }
    }

    private static Memory NewMemory() => Memory.Create(
        new MemoryId(Guid.CreateVersion7()),
        Guid.CreateVersion7(),
        new MemoryPlace("osm:N:1", "Giewont", 49, 19),
        MemoryDate.Create("Day", "2026-07-01", null, new DateOnly(2026, 11, 3)).Value,
        Now);

    private async Task<(int Sequence, DateTimeOffset WindowEnd)> TrashAsync(MemoryId memory, Guid account)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<TrashMemoryHandler>().HandleAsync(new TrashMemory(memory, account), Cancellation);
        var stored = (await scope.ServiceProvider.GetRequiredService<IMemories>().FindAsync(memory, Cancellation))!;
        return (stored.Sequence, stored.WindowEndsAt!.Value);
    }

    private async Task<SharedKernel.Results.Result> RestoreAsync(MemoryId memory, Guid account)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<RestoreMemoryHandler>().HandleAsync(new RestoreMemory(memory, account), Cancellation);
    }

    private async Task<SharedKernel.Results.Result> DeleteAsync(MemoryId memory, int sequence, DateTimeOffset at)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<DeleteMemoryHandler>().HandleAsync(new DeleteMemory(memory, sequence, at), Cancellation);
    }
}
