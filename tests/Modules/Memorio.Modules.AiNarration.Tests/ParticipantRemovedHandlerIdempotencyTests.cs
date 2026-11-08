using Memorio.Modules.AiNarration.Infrastructure.Messaging;
using Memorio.Modules.Album.Contracts;

namespace Memorio.Modules.AiNarration.Tests;

/// <summary>MEM-E6-NARRACJA-AI-02, D-335 (SD6): the consent lapses with the participation, once.</summary>
public sealed class ParticipantRemovedHandlerIdempotencyTests(AiNarrationDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_consent_for_that_album_lapses_once_and_other_albums_keep_theirs()
    {
        var account = Guid.CreateVersion7();
        var album = Guid.CreateVersion7();
        var otherAlbum = Guid.CreateVersion7();
        await RevokeAiConsentTests.GiveAsync(database, account, album);
        await RevokeAiConsentTests.GiveAsync(database, account, otherAlbum);
        var ended = new ParticipantRemovedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, album, account, 5);

        for (var delivery = 0; delivery < 2; delivery++)
        {
            await using var scope = database.CreateScope();
            await ParticipantRemovedHandler.Handle(ended, scope.ServiceProvider, Cancellation);
        }

        Assert.False(await RevokeAiConsentTests.IsGivenAsync(database, account, album));
        Assert.True(await RevokeAiConsentTests.IsGivenAsync(database, account, otherAlbum));
        Assert.Equal(1, await database.QueuedEventsAsync("AiConsentRevokedEvent", account));
    }
}
