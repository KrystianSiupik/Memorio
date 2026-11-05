using Memorio.Modules.Archive.Application.Queries.MemoryDetails;
using Memorio.Modules.Archive.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Archive.Tests;

/// <summary>S2:C8 on a real database: the contact reads what the owner kept private, and nothing in an album.</summary>
public sealed class LegacyArchiveTests(ArchiveDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task A_private_memory_opens_for_the_contact_and_for_nobody_else()
    {
        var (owner, contact, stranger) = (Guid.CreateVersion7(), Guid.CreateVersion7(), Guid.CreateVersion7());
        var memory = await database.AddMemoryAsync(owner);
        await using var scope = database.CreateScope();
        var details = scope.ServiceProvider.GetRequiredService<IMemoryDetailsQuery>();

        Assert.Null(await details.GetAsync(memory.Id.Value, contact, Cancellation));

        database.Neighbours.IsLegacyContact(contact, owner);

        Assert.NotNull(await details.GetAsync(memory.Id.Value, contact, Cancellation));
        Assert.Null(await details.GetAsync(memory.Id.Value, stranger, Cancellation));
    }

    [Fact]
    public async Task What_is_in_an_album_stays_with_the_album()
    {
        var (owner, contact) = (Guid.CreateVersion7(), Guid.CreateVersion7());
        var memory = await database.AddMemoryAsync(owner);
        database.Neighbours.IsLegacyContact(contact, owner);
        await using var scope = database.CreateScope();
        await scope.ServiceProvider.GetRequiredService<IAlbumContent>().LinkAsync(memory.Id.Value, Guid.CreateVersion7(), Cancellation);

        Assert.Null(await scope.ServiceProvider.GetRequiredService<IMemoryDetailsQuery>().GetAsync(memory.Id.Value, contact, Cancellation));
    }

    [Fact]
    public async Task What_is_in_the_trash_is_not_read()
    {
        var (owner, contact) = (Guid.CreateVersion7(), Guid.CreateVersion7());
        var memory = await database.AddMemoryAsync(owner);
        database.Neighbours.IsLegacyContact(contact, owner);
        await database.TrashAsync(memory.Id, owner);

        await using var scope = database.CreateScope();
        Assert.Null(await scope.ServiceProvider.GetRequiredService<IMemoryDetailsQuery>().GetAsync(memory.Id.Value, contact, Cancellation));
    }
}
