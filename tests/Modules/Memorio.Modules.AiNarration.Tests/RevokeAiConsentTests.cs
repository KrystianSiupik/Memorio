using Memorio.Modules.AiNarration.Application.Commands.Consents;
using Memorio.Modules.AiNarration.Domain.Consents;
using Memorio.SharedKernel.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.AiNarration.Tests;

/// <summary>MEM-E6-NARRACJA-AI-02 on a real database: C81 and E81.</summary>
public sealed class RevokeAiConsentTests(AiNarrationDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    internal static async Task GiveAsync(AiNarrationDatabase database, Guid account, Guid album)
    {
        database.Albums.Join(account, album);
        await using var scope = database.CreateScope();
        Assert.True((await scope.ServiceProvider.GetRequiredService<GiveAiConsentHandler>().HandleAsync(new GiveAiConsent(account, album), Cancellation)).IsSuccess);
    }

    internal static async Task<bool> IsGivenAsync(AiNarrationDatabase database, Guid account, Guid album)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<AiConsentQuery>().IsGivenAsync(account, album, Cancellation);
    }

    private async Task<Result> RevokeAsync(Guid account, Guid album)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<RevokeAiConsentHandler>().HandleAsync(new RevokeAiConsent(account, album), Cancellation);
    }

    [Fact]
    public async Task Taken_back_it_is_no_longer_given_and_E81_goes_out_once()
    {
        var account = Guid.CreateVersion7();
        var album = Guid.CreateVersion7();
        await GiveAsync(database, account, album);

        Assert.True((await RevokeAsync(account, album)).IsSuccess);
        Assert.True((await RevokeAsync(account, album)).IsSuccess);

        Assert.False(await IsGivenAsync(database, account, album));
        Assert.Equal(1, await database.QueuedEventsAsync("AiConsentRevokedEvent", account));
    }

    [Fact]
    public async Task Nothing_given_nothing_changes_and_it_can_be_given_again()
    {
        var account = Guid.CreateVersion7();
        var album = Guid.CreateVersion7();

        Assert.True((await RevokeAsync(account, album)).IsSuccess);
        Assert.Equal(ConsentErrors.InvalidAlbum, (await RevokeAsync(account, Guid.Empty)).Error);
        Assert.Equal(0, await database.QueuedEventsAsync("AiConsentRevokedEvent", account));

        await GiveAsync(database, account, album);
        await RevokeAsync(account, album);
        await GiveAsync(database, account, album);
        Assert.True(await IsGivenAsync(database, account, album));
    }

    [Fact]
    public void Revoked_once_with_its_reason()
    {
        var consent = AiConsent.GiveFirst(Guid.CreateVersion7(), Guid.CreateVersion7(), DateTimeOffset.UtcNow);
        consent.ClearDomainEvents();

        Assert.True(consent.Revoke(AiConsentEnd.LeftAlbum, DateTimeOffset.UtcNow));
        Assert.False(consent.Revoke(AiConsentEnd.ByPerson, DateTimeOffset.UtcNow));

        Assert.Equal(AiConsentEnd.LeftAlbum, Assert.IsType<AiConsentRevoked>(Assert.Single(consent.DomainEvents)).Reason);
        Assert.Equal(2, consent.Sequence);
    }
}
