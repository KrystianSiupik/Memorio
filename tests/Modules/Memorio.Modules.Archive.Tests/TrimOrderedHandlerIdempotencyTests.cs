using Memorio.Modules.Archive.Application.Commands.RestoreMemory;
using Memorio.Modules.Archive.Application.Queries.TrimMaterial;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Archive.Domain.Memories;
using Memorio.Modules.Archive.Infrastructure.Messaging;
using Memorio.Modules.Plans.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>SD6: E103 delivered twice, on a real database, with what the trim really takes.</summary>
public sealed class TrimOrderedHandlerIdempotencyTests(ArchiveDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task DeliverAsync(TrimOrderedEvent order)
    {
        await using var scope = database.CreateScope();
        await TrimOrderedHandler.Handle(order, scope.ServiceProvider, Cancellation);
    }

    private static TrimOrderedEvent Order(Guid resource, long limit, IReadOnlyList<Guid>? kept = null, int sequence = 5, bool isAlbum = false) =>
        new(Guid.CreateVersion7(), DateTimeOffset.UtcNow, resource, isAlbum, limit, 0, kept ?? [], sequence);

    [Fact]
    public async Task The_excess_goes_to_the_trash_once_oldest_stays()
    {
        var author = Guid.CreateVersion7();
        var older = await database.AddMemoryAsync(author, dateFrom: new DateOnly(1990, 1, 1), photoBytes: 100);
        var newer = await database.AddMemoryAsync(author, dateFrom: new DateOnly(2020, 1, 1), photoBytes: 100);
        var order = Order(author, 100);

        await DeliverAsync(order);
        await DeliverAsync(order);

        Assert.Equal(MemoryState.Active, (await database.FindAsync(older.Id))!.State);
        var trimmed = (await database.FindAsync(newer.Id))!;
        Assert.Equal(MemoryState.Trashed, trimmed.State);
        Assert.Equal(TrashReason.Trimmed, trimmed.TrashReason);
        Assert.Equal(author, trimmed.TrimmedFor);
        Assert.True(await ArchiveDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(ArchiveTrimmedEvent), author) == 1));
        Assert.True(await ArchiveDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(MemoryMovedToTrashEvent), newer.Id.Value) == 1));
    }

    [Fact]
    public async Task The_choice_of_the_owner_stays_first()
    {
        var author = Guid.CreateVersion7();
        var older = await database.AddMemoryAsync(author, dateFrom: new DateOnly(1990, 1, 1), photoBytes: 100);
        var chosen = await database.AddMemoryAsync(author, dateFrom: new DateOnly(2020, 1, 1), photoBytes: 100);

        await DeliverAsync(Order(author, 100, [chosen.Id.Value]));

        Assert.Equal(MemoryState.Trashed, (await database.FindAsync(older.Id))!.State);
        Assert.Equal(MemoryState.Active, (await database.FindAsync(chosen.Id))!.State);
    }

    [Fact]
    public async Task A_late_order_is_skipped()
    {
        var author = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(author, dateFrom: new DateOnly(2020, 1, 1), photoBytes: 100);
        await using (var scope = database.CreateScope())
        {
            await PlanActivatedHandler.Handle(new PlanActivatedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, author, false, 1000, 1, 9), scope.ServiceProvider, Cancellation);
        }

        await DeliverAsync(Order(author, 0, sequence: 8));

        Assert.Equal(MemoryState.Active, (await database.FindAsync(memory.Id))!.State);
    }

    [Fact]
    public async Task A_memory_in_an_album_is_not_trimmed_with_the_account_of_its_author()
    {
        var author = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(author, photoBytes: 100);
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<IAlbumContent>().LinkAsync(memory.Id.Value, Guid.CreateVersion7(), Cancellation);

        await DeliverAsync(Order(author, 0));

        Assert.Equal(MemoryState.Active, (await database.FindAsync(memory.Id))!.State);
    }

    [Fact]
    public async Task An_album_loses_only_what_is_in_no_other_album()
    {
        var album = Guid.CreateVersion7();
        var alone = await database.AddMemoryAsync(Guid.CreateVersion7(), photoBytes: 100);
        var shared = await database.AddMemoryAsync(Guid.CreateVersion7(), photoBytes: 100);
        var elsewhere = await database.AddMemoryAsync(Guid.CreateVersion7(), photoBytes: 100);
        await using var scope = database.CreateScope();
        var content = scope.ServiceProvider.GetRequiredService<IAlbumContent>();
        await content.LinkAsync(alone.Id.Value, album, Cancellation);
        await content.LinkAsync(shared.Id.Value, album, Cancellation);
        await content.LinkAsync(shared.Id.Value, Guid.CreateVersion7(), Cancellation);

        var material = await content.TrimMaterialAsync(album, Cancellation);
        Assert.Equal(alone.Id.Value, Assert.Single(material).MemoryId);
        Assert.Equal(100, material[0].SizeBytes);

        await DeliverAsync(Order(album, 0, isAlbum: true));

        Assert.Equal(MemoryState.Trashed, (await database.FindAsync(alone.Id))!.State);
        Assert.Equal(MemoryState.Active, (await database.FindAsync(shared.Id))!.State);
        Assert.Equal(MemoryState.Active, (await database.FindAsync(elsewhere.Id))!.State);
    }

    [Fact]
    public async Task The_list_for_the_owner_is_oldest_first_with_sizes()
    {
        var author = Guid.CreateVersion7();
        var newer = await database.AddMemoryAsync(author, dateFrom: new DateOnly(2020, 1, 1), photoBytes: 7);
        var older = await database.AddMemoryAsync(author, dateFrom: new DateOnly(1990, 1, 1));
        await using var scope = database.CreateScope();

        var items = await scope.ServiceProvider.GetRequiredService<ITrimMaterialQuery>().OfAccountAsync(author, Cancellation);

        Assert.Equal([older.Id.Value, newer.Id.Value], items.Select(item => item.MemoryId));
        Assert.Equal([0L, 7L], items.Select(item => item.SizeBytes));
        Assert.Equal("1990-01-01", items[0].DateFrom);
    }

    [Fact]
    public async Task What_was_trimmed_is_not_restored_by_the_person_without_a_plan()
    {
        var author = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(author, photoBytes: 100);
        await DeliverAsync(Order(author, 0));

        await using var scope = database.CreateScope();
        var restored = await scope.ServiceProvider.GetRequiredService<RestoreMemoryHandler>().HandleAsync(new RestoreMemory(memory.Id, author), Cancellation);

        Assert.Equal(MemoryErrors.PlanRequired, restored.Error);
    }
}

/// <summary>SD6: E94 delivered twice: what was trimmed comes back within its 30 days (P44, HS-022).</summary>
public sealed class PlanActivatedHandlerIdempotencyTests(ArchiveDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task TrimAsync(Guid resource, int sequence = 5)
    {
        await using var scope = database.CreateScope();
        await TrimOrderedHandler.Handle(new TrimOrderedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, resource, false, 0, 0, [], sequence), scope.ServiceProvider, Cancellation);
    }

    private async Task PayAsync(PlanActivatedEvent paid)
    {
        await using var scope = database.CreateScope();
        await PlanActivatedHandler.Handle(paid, scope.ServiceProvider, Cancellation);
    }

    private static PlanActivatedEvent Paid(Guid resource, int sequence = 6) =>
        new(Guid.CreateVersion7(), DateTimeOffset.UtcNow, resource, false, 1000, 1, sequence);

    [Fact]
    public async Task Paying_again_brings_it_back_once()
    {
        var author = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(author, photoBytes: 100);
        await TrimAsync(author);
        var paid = Paid(author);

        await PayAsync(paid);
        await PayAsync(paid);

        var restored = (await database.FindAsync(memory.Id))!;
        Assert.Equal(MemoryState.Active, restored.State);
        Assert.Null(restored.TrimmedFor);
        Assert.True(await ArchiveDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(MemoryRestoredEvent), memory.Id.Value) == 1));
    }

    [Fact]
    public async Task What_the_person_trashed_stays_in_the_trash()
    {
        var author = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(author);
        await database.TrashAsync(memory.Id, author);

        await PayAsync(Paid(author));

        Assert.Equal(MemoryState.Trashed, (await database.FindAsync(memory.Id))!.State);
    }

    [Fact]
    public async Task An_ended_window_stays_ended_and_the_others_come_back()
    {
        var author = Guid.CreateVersion7();
        var ended = await database.AddMemoryAsync(author, photoBytes: 100);
        var open = await database.AddMemoryAsync(author, photoBytes: 100);
        await TrimAsync(author);
        await using (var connection = await database.OpenAsAsync(ArchiveDatabase.Role))
        {
            await using var command = new Npgsql.NpgsqlCommand("UPDATE archive.memories SET window_ends_at = now() - interval '1 hour' WHERE id = @id", connection);
            command.Parameters.AddWithValue("id", ended.Id.Value);
            await command.ExecuteNonQueryAsync(Cancellation);
        }

        await PayAsync(Paid(author));

        Assert.Equal(MemoryState.Trashed, (await database.FindAsync(ended.Id))!.State);
        Assert.Equal(MemoryState.Active, (await database.FindAsync(open.Id))!.State);
    }

    [Fact]
    public async Task A_late_activation_restores_nothing()
    {
        var author = Guid.CreateVersion7();
        var memory = await database.AddMemoryAsync(author, photoBytes: 100);
        await TrimAsync(author, sequence: 5);

        await PayAsync(Paid(author, sequence: 4));

        Assert.Equal(MemoryState.Trashed, (await database.FindAsync(memory.Id))!.State);
    }
}
