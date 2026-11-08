using Memorio.Modules.AiNarration.Application.Commands.Consents;
using Memorio.Modules.AiNarration.Domain.Consents;
using Memorio.SharedKernel.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.AiNarration.Tests;

/// <summary>MEM-E6-NARRACJA-AI-01 on a real database: C80, the consent per person and album (S2:C5).</summary>
public sealed class AiConsentTests(AiNarrationDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task<Result> GiveAsync(Guid account, Guid album)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<GiveAiConsentHandler>().HandleAsync(new GiveAiConsent(account, album), Cancellation);
    }

    private async Task<bool> IsGivenAsync(Guid account, Guid album)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<AiConsentQuery>().IsGivenAsync(account, album, Cancellation);
    }

    [Fact]
    public async Task A_participant_gives_it_for_one_album_only_and_again_changes_nothing()
    {
        var account = Guid.CreateVersion7();
        var album = Guid.CreateVersion7();
        var otherAlbum = Guid.CreateVersion7();
        database.Albums.Join(account, album);
        database.Albums.Join(account, otherAlbum);

        Assert.True((await GiveAsync(account, album)).IsSuccess);
        Assert.True((await GiveAsync(account, album)).IsSuccess);

        Assert.True(await IsGivenAsync(account, album));
        Assert.False(await IsGivenAsync(account, otherAlbum));
        Assert.Equal(1, await database.QueuedEventsAsync("AiConsentGivenEvent", account));
    }

    [Fact]
    public async Task Somebody_outside_the_album_and_a_missing_album_are_refused_without_a_record()
    {
        var account = Guid.CreateVersion7();
        var album = Guid.CreateVersion7();

        Assert.Equal(ConsentErrors.NotMember, (await GiveAsync(account, album)).Error);
        Assert.Equal(ConsentErrors.InvalidAlbum, (await GiveAsync(account, Guid.Empty)).Error);

        Assert.False(await IsGivenAsync(account, album));
        Assert.Equal(0, await database.QueuedEventsAsync("AiConsentGivenEvent", account));
    }

    [Fact]
    public async Task Given_twice_at_once_it_is_one_consent()
    {
        var account = Guid.CreateVersion7();
        var album = Guid.CreateVersion7();
        database.Albums.Join(account, album);

        var results = await Task.WhenAll(GiveAsync(account, album), GiveAsync(account, album), GiveAsync(account, album));

        Assert.All(results, result => Assert.True(result.IsSuccess));
        Assert.True(await IsGivenAsync(account, album));
        Assert.Equal(1, await database.QueuedEventsAsync("AiConsentGivenEvent", account));
    }

    [Fact]
    public void The_consent_is_raised_once()
    {
        var consent = AiConsent.GiveFirst(Guid.CreateVersion7(), Guid.CreateVersion7(), DateTimeOffset.UtcNow);

        Assert.False(consent.Give(DateTimeOffset.UtcNow));
        Assert.True(consent.IsGiven);
        Assert.Equal(1, consent.Sequence);
        Assert.IsType<AiConsentGiven>(Assert.Single(consent.DomainEvents));
    }
}
