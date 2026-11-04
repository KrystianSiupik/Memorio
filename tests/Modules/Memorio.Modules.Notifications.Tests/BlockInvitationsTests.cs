using Memorio.Modules.Notifications.Application.Commands;
using Memorio.Modules.Notifications.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Notifications.Tests;

/// <summary>MEM-E3-POWIADOMIENIA-02 and -06 on a real database.</summary>
public sealed class BlockInvitationsTests(NotificationsDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_address_is_blocked_whatever_its_spelling_and_asking_again_changes_nothing()
    {
        var person = database.Neighbours.NewAccount();
        await using var scope = database.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<BlockInvitationsHandler>();
        var blocks = scope.ServiceProvider.GetRequiredService<IInvitationBlocks>();
        var email = database.Neighbours.EmailOf(person);
        Assert.False(await blocks.IsBlockedAsync(email, Cancellation));

        Assert.True((await handler.HandleAsync(person, Cancellation)).IsSuccess);
        Assert.True((await handler.HandleAsync(person, Cancellation)).IsSuccess);

        Assert.True(await blocks.IsBlockedAsync(email, Cancellation));
        Assert.True(await blocks.IsBlockedAsync(" " + email.ToUpperInvariant() + " ", Cancellation));
    }

    [Fact]
    public async Task No_account_no_block()
    {
        await using var scope = database.CreateScope();

        var refused = await scope.ServiceProvider.GetRequiredService<BlockInvitationsHandler>().HandleAsync(Guid.CreateVersion7(), Cancellation);

        Assert.Equal(BlockInvitationsHandler.UnknownAccount, refused.Error);
    }
}
