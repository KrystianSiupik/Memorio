using Memorio.Modules.Album.Application.Commands.Contributions;
using Memorio.Modules.Album.Application.Commands.CreateAlbum;
using Memorio.Modules.Album.Domain.Albums;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Tests;

/// <summary>E95 on a real database: everyone keeps reading, and nothing that adds to the album works (S2:B1).</summary>
public sealed class AlbumReadOnlyTests(AlbumDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private static string NewAddress() => $"{Guid.NewGuid()}@example.invalid";

    [Fact]
    public async Task No_invitation_and_no_memory_and_no_message_to_the_owner()
    {
        var (album, owner, participant) = await database.AlbumWithParticipantAsync();
        var memory = database.Neighbours.NewMemory(participant);
        database.Neighbours.ReadOnly(album.Value);

        Assert.Equal(AlbumErrors.ReadOnly, (await database.InviteAsync(album, owner, NewAddress())).Error);
        Assert.Equal(AlbumErrors.ReadOnly, (await database.AddMemoryAsync(album, participant, memory)).Error);

        Assert.Empty((await database.SeatsAsync(album, owner)).Invited);
        await Task.Delay(300, Cancellation);
        Assert.Equal(0, await database.QueuedEventsAsync("AlbumPoolExhaustedEvent", album.Value));
    }

    [Fact]
    public async Task No_photo_either()
    {
        var (album, _, participant) = await database.AlbumWithParticipantAsync();
        var memory = await database.BringMemoryAsync(album, participant);
        database.Neighbours.ReadOnly(album.Value);

        await using var scope = database.CreateScope();
        var added = await scope.ServiceProvider.GetRequiredService<AddPhotoInAlbumHandler>().HandleAsync(
            new AddPhotoInAlbum(album, participant, memory, Guid.CreateVersion7(), "upload"),
            Cancellation);

        Assert.Equal(AlbumErrors.ReadOnly, added.Error);
    }

    [Fact]
    public async Task Bringing_what_is_already_there_is_still_no_error()
    {
        var (album, _, participant) = await database.AlbumWithParticipantAsync();
        var memory = await database.BringMemoryAsync(album, participant);
        database.Neighbours.ReadOnly(album.Value);

        Assert.True((await database.AddMemoryAsync(album, participant, memory)).IsSuccess);
    }

    [Fact]
    public async Task An_album_cannot_take_the_id_of_an_account()
    {
        var victim = database.Neighbours.NewAccount();
        var attacker = database.Neighbours.NewAccount();

        await using var scope = database.CreateScope();
        var created = await scope.ServiceProvider.GetRequiredService<CreateAlbumHandler>().HandleAsync(new CreateAlbum(victim, attacker, "Rodzina"), Cancellation);

        Assert.Equal(AlbumErrors.IdTaken, created.Error);
    }
}
