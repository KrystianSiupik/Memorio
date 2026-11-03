using Memorio.Modules.Archive.Application.Commands.AddPhoto;
using Memorio.Modules.Archive.Application.Commands.DeleteMemory;
using Memorio.Modules.Archive.Application.Commands.TrashMemory;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Archive.Domain.Memories;
using Memorio.SharedKernel.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>MEM-E2-ARCHIWUM-05: AG10 without a database, then the whole way on a real one.</summary>
public sealed class DeleteMemoryTests(ArchiveDatabase database)
{
    private static readonly DateTimeOffset Now = new(2026, 11, 3, 12, 0, 0, TimeSpan.Zero);

    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public void A_window_passes_only_for_its_sequence_while_open_and_after_its_end()
    {
        var memory = NewMemory();
        Assert.Equal(MemoryErrors.AlreadyDecided, memory.CanLeaveTrash(1, Now.AddDays(40)).Error);

        memory.MoveToTrash(Now);

        Assert.Equal(MemoryErrors.TrashWindowOpen, memory.CanLeaveTrash(memory.Sequence, Now.AddDays(30).AddTicks(-1)).Error);
        Assert.Equal(MemoryErrors.AlreadyDecided, memory.CanLeaveTrash(memory.Sequence - 1, Now.AddDays(30)).Error);
        Assert.True(memory.CanLeaveTrash(memory.Sequence, Now.AddDays(30)).IsSuccess);
    }

    [Fact]
    public void Deleting_gives_both_events()
    {
        var memory = NewMemory();
        memory.ClearDomainEvents();

        memory.DeleteDefinitively(Now);

        Assert.Single(memory.DomainEvents.OfType<ContentDeletedDefinitively>());
        Assert.Single(memory.DomainEvents.OfType<MemoryCeasedToExist>());
    }

    [Fact]
    public async Task After_the_window_the_row_the_files_and_the_space_are_gone()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        var photo = Guid.CreateVersion7();
        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<AddPhotoHandler>()
                .HandleAsync(new AddPhoto(memory.Id, account, photo, database.Storage.AddUpload(50).Value), Cancellation);
        }

        var (sequence, windowEnd) = await TrashAsync(memory.Id, account);

        var deleted = await DeleteAsync(memory.Id, sequence, windowEnd);

        Assert.True(deleted.IsSuccess);
        Assert.False(database.Neighbours.Reserved.ContainsKey(photo));
        Assert.Contains(new StorageKey($"photos/{photo}/original"), database.Storage.Deleted);
        await using var check = database.CreateScope();
        Assert.Equal(new AccountContentSummary(0, 0), await check.ServiceProvider.GetRequiredService<IAccountContentReader>().SummarizeAsync(account, Cancellation));
    }

    [Fact]
    public async Task The_second_decision_about_a_window_is_refused()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        var (sequence, windowEnd) = await TrashAsync(memory.Id, account);

        await DeleteAsync(memory.Id, sequence, windowEnd);
        var again = await DeleteAsync(memory.Id, sequence, windowEnd);

        Assert.Equal(MemoryErrors.AlreadyDecided, again.Error);
    }

    [Fact]
    public async Task Parallel_deletions_have_one_winner()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        var (sequence, windowEnd) = await TrashAsync(memory.Id, account);

        var results = await Task.WhenAll(Enumerable.Range(0, 5).Select(_ => DeleteAsync(memory.Id, sequence, windowEnd)));

        Assert.Single(results, result => result.IsSuccess);
        Assert.All(results.Where(result => result.IsFailure), result => Assert.Equal(MemoryErrors.AlreadyDecided, result.Error));
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

    private async Task<SharedKernel.Results.Result> DeleteAsync(MemoryId memory, int sequence, DateTimeOffset at)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<DeleteMemoryHandler>().HandleAsync(new DeleteMemory(memory, sequence, at), Cancellation);
    }
}
