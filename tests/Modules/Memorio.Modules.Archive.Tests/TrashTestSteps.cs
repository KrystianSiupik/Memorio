using Memorio.Modules.Archive.Application.Commands.RestoreMemory;
using Memorio.Modules.Archive.Application.Commands.TrashMemory;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Archive.Domain.Memories;
using Memorio.Modules.Archive.Infrastructure.Messaging;
using Memorio.SharedKernel.Scheduling;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

internal static class TrashTestSteps
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    public static async Task<Memory> TrashAsync(this ArchiveDatabase database, MemoryId id, Guid account)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<TrashMemoryHandler>().HandleAsync(new TrashMemory(id, account), Cancellation);
        return (await scope.ServiceProvider.GetRequiredService<IMemories>().FindAsync(id, Cancellation))!;
    }

    public static async Task RestoreAsync(this ArchiveDatabase database, MemoryId id, Guid account)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<RestoreMemoryHandler>().HandleAsync(new RestoreMemory(id, account), Cancellation);
    }

    public static async Task DeliverAsync(this ArchiveDatabase database, MemoryMovedToTrashEvent moved)
    {
        await using var scope = database.CreateScope();
        await MemoryMovedToTrashHandler.Handle(moved, scope.ServiceProvider, Cancellation);
    }

    public static async Task DeliverAsync(this ArchiveDatabase database, MemoryRestoredEvent restored)
    {
        await using var scope = database.CreateScope();
        await MemoryRestoredHandler.Handle(restored, scope.ServiceProvider, Cancellation);
    }

    public static async Task FireAsync(this ArchiveDatabase database, ScheduledJobRun run)
    {
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredKeyedService<IScheduledJob>(run.Job).RunAsync(run, Cancellation);
    }

    public static async Task<Memory?> FindAsync(this ArchiveDatabase database, MemoryId id)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IMemories>().FindAsync(id, Cancellation);
    }

    public static MemoryMovedToTrashEvent MovedEvent(Memory memory) =>
        new(Guid.CreateVersion7(), DateTimeOffset.UtcNow, memory.Id.Value, memory.Sequence, memory.WindowEndsAt!.Value);
}
