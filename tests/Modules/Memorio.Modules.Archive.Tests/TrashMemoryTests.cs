using Memorio.Modules.Archive.Application.Commands.AddPhoto;
using Memorio.Modules.Archive.Application.Commands.TrashMemory;
using Memorio.Modules.Archive.Application.Queries.MemoryDetails;
using Memorio.Modules.Archive.Domain.Memories;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>MEM-E2-ARCHIWUM-04 on a real database.</summary>
public sealed class TrashMemoryTests(ArchiveDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public void The_trash_lasts_30_days_and_gives_the_event()
    {
        var now = new DateTimeOffset(2026, 11, 3, 12, 0, 0, TimeSpan.Zero);
        var memory = NewMemory(now);
        memory.ClearDomainEvents();

        memory.MoveToTrash(now);

        Assert.Equal(MemoryState.Trashed, memory.State);
        Assert.Equal(now.AddDays(30), memory.WindowEndsAt);
        var moved = Assert.IsType<MemoryMovedToTrash>(Assert.Single(memory.DomainEvents));
        Assert.Equal(2, moved.Sequence);
    }

    [Fact]
    public void Asking_again_keeps_the_window()
    {
        var now = new DateTimeOffset(2026, 11, 3, 12, 0, 0, TimeSpan.Zero);
        var memory = NewMemory(now);
        memory.MoveToTrash(now);
        memory.ClearDomainEvents();

        memory.MoveToTrash(now.AddDays(5));

        Assert.Equal(now.AddDays(30), memory.WindowEndsAt);
        Assert.Empty(memory.DomainEvents);
    }

    [Fact]
    public async Task In_the_trash_it_is_gone_from_the_details_and_takes_no_photos()
    {
        var account = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(account);

        var trashed = await TrashAsync(memory.Id, account);

        Assert.True(trashed.IsSuccess);
        await using var scope = database.CreateScope();
        Assert.Null(await scope.ServiceProvider.GetRequiredService<IMemoryDetailsQuery>().GetAsync(memory.Id.Value, account, Cancellation));
        var photo = await scope.ServiceProvider.GetRequiredService<AddPhotoHandler>()
            .HandleAsync(new AddPhoto(memory.Id, account, Guid.CreateVersion7(), database.Storage.AddUpload(1).Value), Cancellation);
        Assert.Equal(MemoryErrors.AlreadyDecided, photo.Error);
    }

    [Fact]
    public async Task Someone_elses_memory_is_refused()
    {
        var memory = await database.AddMemoryAsync(Guid.CreateVersion7());

        var refused = await TrashAsync(memory.Id, Guid.CreateVersion7());

        Assert.Equal(MemoryErrors.NoAccess, refused.Error);
    }

    private static Memory NewMemory(DateTimeOffset now) => Memory.Create(
        new MemoryId(Guid.CreateVersion7()),
        Guid.CreateVersion7(),
        new MemoryPlace("osm:N:1", "Giewont", 49, 19),
        MemoryDate.Create("Day", "2026-07-01", null, new DateOnly(2026, 11, 3)).Value,
        now);

    private async Task<SharedKernel.Results.Result> TrashAsync(MemoryId memory, Guid account)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<TrashMemoryHandler>().HandleAsync(new TrashMemory(memory, account), Cancellation);
    }
}
