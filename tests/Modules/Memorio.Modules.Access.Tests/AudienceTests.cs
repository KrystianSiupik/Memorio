using Memorio.Modules.Access.Application.Commands;
using Memorio.Modules.Access.Application.Queries;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Access.Domain.Audiences;
using Memorio.SharedKernel.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Access.Tests;

/// <summary>MEM-E4-DOSTEP-01 to -08 on a real database.</summary>
public sealed class AudienceTests(AccessDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private static readonly DateTimeOffset Now = new(2026, 11, 5, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void A_working_link_stays_the_same_and_a_revoked_one_lets_nobody_new_in()
    {
        var audience = AlbumAudience.For(Guid.CreateVersion7());
        var (first, second) = (Guid.CreateVersion7(), Guid.CreateVersion7());

        Assert.Equal("a", audience.GenerateLink("a", Now));
        Assert.Equal("a", audience.GenerateLink("b", Now));
        Assert.True(audience.Enter("a", first, Now).IsSuccess);
        Assert.True(audience.RevokeLink(Now).IsSuccess);

        Assert.Equal(AccessErrors.LinkRevoked, audience.Enter("a", second, Now).Error);
        Assert.True(audience.IsViewer(first));
        Assert.Equal(AccessErrors.AlreadyDecided, audience.RevokeLink(Now).Error);
    }

    [Fact]
    public void Whose_access_was_taken_may_come_back_but_a_blocked_person_may_not()
    {
        var audience = AlbumAudience.For(Guid.CreateVersion7());
        var person = Guid.CreateVersion7();
        audience.GenerateLink("a", Now);
        audience.Enter("a", person, Now);

        Assert.True(audience.RevokeAccess(person, Now).IsSuccess);
        Assert.False(audience.IsViewer(person));
        Assert.Equal(AccessErrors.AlreadyDecided, audience.RevokeAccess(person, Now).Error);
        Assert.True(audience.Enter("a", person, Now).IsSuccess);

        audience.Block(person, Now);
        audience.ClearDomainEvents();
        audience.Block(person, Now);

        Assert.Empty(audience.DomainEvents);
        Assert.False(audience.IsViewer(person));
        Assert.Equal(AccessErrors.Blocked, audience.Enter("a", person, Now).Error);
        Assert.Equal(AccessErrors.Blocked, audience.GrantRead(person, Now).Error);
    }

    [Fact]
    public async Task Only_the_owner_shares_and_revokes_and_a_new_link_is_another_link()
    {
        var (album, owner) = database.Neighbours.NewAlbum();
        var participant = database.Neighbours.NewParticipant(album);
        await using var scope = database.CreateScope();
        var share = scope.ServiceProvider.GetRequiredService<ShareByLinkHandler>();
        var revoke = scope.ServiceProvider.GetRequiredService<RevokeLinkHandler>();

        Assert.Equal(AccessErrors.NoAccess, (await share.HandleAsync(album, participant, Cancellation)).Error);
        Assert.Equal(AccessErrors.NoAccess, (await share.HandleAsync(Guid.CreateVersion7(), owner, Cancellation)).Error);

        var first = (await share.HandleAsync(album, owner, Cancellation)).Value;
        Assert.Equal(64, first.Length);
        Assert.Equal(first, (await share.HandleAsync(album, owner, Cancellation)).Value);
        Assert.Equal(first, await scope.ServiceProvider.GetRequiredService<IAudienceQueries>().LinkTokenAsync(album, Cancellation));

        Assert.Equal(AccessErrors.NoAccess, (await revoke.HandleAsync(album, participant, Cancellation)).Error);
        Assert.True((await revoke.HandleAsync(album, owner, Cancellation)).IsSuccess);
        Assert.Null(await scope.ServiceProvider.GetRequiredService<IAudienceQueries>().AlbumOfTokenAsync(first, Cancellation));
        Assert.NotEqual(first, (await share.HandleAsync(album, owner, Cancellation)).Value);
    }

    [Fact]
    public async Task Entering_is_by_name_once_and_someone_in_the_album_takes_no_place_on_the_list()
    {
        var (album, owner) = database.Neighbours.NewAlbum();
        var participant = database.Neighbours.NewParticipant(album);
        var person = Guid.CreateVersion7();
        var token = await ShareAsync(album, owner);

        Assert.Equal(album, (await EnterAsync(token, person)).Value);
        Assert.Equal(album, (await EnterAsync(token, person)).Value);
        Assert.Equal(album, (await EnterAsync(token, participant)).Value);
        Assert.Equal(album, (await EnterAsync(token, owner)).Value);

        await using var scope = database.CreateScope();
        Assert.Equal(person, Assert.Single(await scope.ServiceProvider.GetRequiredService<IAudienceQueries>().ViewersAsync(album, Cancellation)).AccountId);
        Assert.Equal(AccessErrors.LinkRevoked, (await EnterAsync("nie-ma-takiego", person)).Error);
    }

    [Fact]
    public async Task Parallel_entries_and_a_revoked_link_never_let_anyone_in_after_the_revoke()
    {
        var (album, owner) = database.Neighbours.NewAlbum();
        var token = await ShareAsync(album, owner);
        var people = Enumerable.Range(0, 6).Select(_ => Guid.CreateVersion7()).ToList();

        async Task<Result> RevokeAsync()
        {
            await using var scope = database.CreateScope();
            return await scope.ServiceProvider.GetRequiredService<RevokeLinkHandler>().HandleAsync(album, owner, Cancellation);
        }

        var entries = people.Select(person => EnterAsync(token, person)).ToList();
        var revoking = RevokeAsync();
        await Task.WhenAll(entries);
        Assert.True((await revoking).IsSuccess);

        var late = await EnterAsync(token, Guid.CreateVersion7());
        Assert.Equal(AccessErrors.LinkRevoked, late.Error);

        await using var check = database.CreateScope();
        var viewers = (await check.ServiceProvider.GetRequiredService<IAudienceQueries>().ViewersAsync(album, Cancellation)).Select(viewer => viewer.AccountId).ToList();
        for (var index = 0; index < people.Count; index++)
        {
            Assert.Equal((await entries[index]).IsSuccess, viewers.Contains(people[index]));
        }
    }

    [Fact]
    public async Task A_parallel_entry_and_block_end_with_the_person_blocked_and_off_the_list()
    {
        var (album, owner) = database.Neighbours.NewAlbum();
        var token = await ShareAsync(album, owner);
        var person = Guid.CreateVersion7();

        async Task BlockAsync()
        {
            await using var scope = database.CreateScope();
            Assert.True((await scope.ServiceProvider.GetRequiredService<BlockUserHandler>().HandleAsync(album, owner, person, Cancellation)).IsSuccess);
        }

        await Task.WhenAll(EnterAsync(token, person), BlockAsync());

        await using var check = database.CreateScope();
        Assert.False(await check.ServiceProvider.GetRequiredService<IAccessReader>().CanSeeAlbumAsync(person, album, Cancellation));
        Assert.DoesNotContain(await check.ServiceProvider.GetRequiredService<IAudienceQueries>().ViewersAsync(album, Cancellation), viewer => viewer.AccountId == person);
        Assert.Equal(AccessErrors.Blocked, (await EnterAsync(token, person)).Error);
    }

    [Fact]
    public async Task Only_the_owner_takes_access_away_and_the_event_goes_through_the_outbox()
    {
        var (album, owner) = database.Neighbours.NewAlbum();
        var participant = database.Neighbours.NewParticipant(album);
        var person = Guid.CreateVersion7();
        await EnterAsync(await ShareAsync(album, owner), person);
        await using var scope = database.CreateScope();
        var revoke = scope.ServiceProvider.GetRequiredService<RevokeAccessHandler>();

        Assert.Equal(AccessErrors.NoAccess, (await revoke.HandleAsync(album, participant, person, Cancellation)).Error);
        Assert.True((await revoke.HandleAsync(album, owner, person, Cancellation)).IsSuccess);
        Assert.Equal(AccessErrors.AlreadyDecided, (await revoke.HandleAsync(album, owner, person, Cancellation)).Error);

        Assert.False(await scope.ServiceProvider.GetRequiredService<IAccessReader>().CanSeeAlbumAsync(person, album, Cancellation));
        Assert.True(await AccessDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync("AccessRevokedEvent", person) == 1));
    }

    [Fact]
    public async Task Who_may_block_whom()
    {
        var (album, owner) = database.Neighbours.NewAlbum();
        var participant = database.Neighbours.NewParticipant(album);
        var another = database.Neighbours.NewParticipant(album);
        var viewer = Guid.CreateVersion7();
        await EnterAsync(await ShareAsync(album, owner), viewer);
        await using var scope = database.CreateScope();
        var block = scope.ServiceProvider.GetRequiredService<BlockUserHandler>();

        Assert.Equal(AccessErrors.NoAccess, (await block.HandleAsync(album, viewer, participant, Cancellation)).Error);
        Assert.Equal(AccessErrors.CannotBlockYourself, (await block.HandleAsync(album, participant, participant, Cancellation)).Error);
        Assert.Equal(AccessErrors.CannotBlockOwner, (await block.HandleAsync(album, participant, owner, Cancellation)).Error);
        Assert.Equal(AccessErrors.OnlyOwnerBlocksParticipant, (await block.HandleAsync(album, participant, another, Cancellation)).Error);

        Assert.True((await block.HandleAsync(album, participant, viewer, Cancellation)).IsSuccess);
        Assert.True((await block.HandleAsync(album, owner, another, Cancellation)).IsSuccess);
        Assert.True((await block.HandleAsync(album, owner, another, Cancellation)).IsSuccess);

        Assert.True(await AccessDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync("UserBlockedEvent", viewer) == 1));
        Assert.True(await AccessDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync("UserBlockedEvent", another) == 1));
    }

    [Fact]
    public async Task A_rule_grants_reading_unless_a_block_stands_in_the_way()
    {
        var (album, owner) = database.Neighbours.NewAlbum();
        var (person, blocked) = (Guid.CreateVersion7(), Guid.CreateVersion7());
        await using var scope = database.CreateScope();
        var grants = scope.ServiceProvider.GetRequiredService<IReadGrants>();
        await scope.ServiceProvider.GetRequiredService<BlockUserHandler>().HandleAsync(album, owner, blocked, Cancellation);

        Assert.True(await grants.GrantAsync(album, person, Cancellation));
        Assert.True(await grants.GrantAsync(album, person, Cancellation));
        Assert.False(await grants.GrantAsync(album, blocked, Cancellation));

        Assert.True(await scope.ServiceProvider.GetRequiredService<IAccessReader>().CanSeeAlbumAsync(person, album, Cancellation));
        Assert.Single(await scope.ServiceProvider.GetRequiredService<IAudienceQueries>().ViewersAsync(album, Cancellation));
    }

    private async Task<string> ShareAsync(Guid album, Guid owner)
    {
        await using var scope = database.CreateScope();
        return (await scope.ServiceProvider.GetRequiredService<ShareByLinkHandler>().HandleAsync(album, owner, Cancellation)).Value;
    }

    private async Task<Result<Guid>> EnterAsync(string token, Guid person)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<EnterByLinkHandler>().HandleAsync(token, person, Cancellation);
    }
}
