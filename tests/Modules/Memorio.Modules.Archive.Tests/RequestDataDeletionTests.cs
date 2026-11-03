using Memorio.Modules.Archive.Application.Commands.DeleteMemory;
using Memorio.Modules.Archive.Application.Commands.TrashMemory;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Archive.Domain.Memories;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>MEM-E2-ARCHIWUM-07 on a real database.</summary>
public sealed class RequestDataDeletionTests(ArchiveDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task It_skips_the_trash_and_gives_the_event()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account, photos: 1);

        var deleted = await RequestAsync(memory.Id, account);

        Assert.True(deleted.IsSuccess);
        Assert.Null(await FindAsync(memory.Id));
        Assert.True(await ArchiveDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(ContentDeleted), memory.Id.Value) == 1));
    }

    [Fact]
    public async Task It_works_from_the_trash_too()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);
        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<TrashMemoryHandler>().HandleAsync(new TrashMemory(memory.Id, account), Cancellation);
        }

        Assert.True((await RequestAsync(memory.Id, account)).IsSuccess);
        Assert.Null(await FindAsync(memory.Id));
    }

    [Fact]
    public async Task Without_the_confirmation_nothing_happens()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);

        var refused = await RequestAsync(memory.Id, account, confirmed: false);

        Assert.Equal(MemoryErrors.ConfirmationRequired, refused.Error);
        Assert.NotNull(await FindAsync(memory.Id));
    }

    [Fact]
    public async Task Someone_elses_memory_is_refused()
    {
        var memory = await database.AddMemoryAsync(Guid.CreateVersion7());

        var refused = await RequestAsync(memory.Id, Guid.CreateVersion7());

        Assert.Equal(MemoryErrors.NoAccess, refused.Error);
        Assert.NotNull(await FindAsync(memory.Id));
    }

    private async Task<SharedKernel.Results.Result> RequestAsync(MemoryId memory, Guid account, bool confirmed = true)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<RequestDataDeletionHandler>().HandleAsync(new RequestDataDeletion(memory, account, confirmed), Cancellation);
    }

    private async Task<Memory?> FindAsync(MemoryId memory)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IMemories>().FindAsync(memory, Cancellation);
    }
}
