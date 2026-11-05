using Memorio.Modules.Access.Application.Commands;
using Memorio.Modules.Access.Application.Policies;
using Memorio.Modules.Access.Application.Queries;
using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Account.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Access.Tests;

/// <summary>SD6: E7 delivered twice, on a real database.</summary>
public sealed class AccountDeletedHandlerIdempotencyTests(AccessDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task A_deleted_account_leaves_every_list_and_others_stay()
    {
        var (album, owner) = database.Neighbours.NewAlbum();
        var (person, other, blocked) = (Guid.CreateVersion7(), Guid.CreateVersion7(), Guid.CreateVersion7());
        await using var scope = database.CreateScope();
        var token = (await scope.ServiceProvider.GetRequiredService<ShareByLinkHandler>().HandleAsync(album, owner, Cancellation)).Value;
        var enter = scope.ServiceProvider.GetRequiredService<EnterByLinkHandler>();
        await enter.HandleAsync(token, person, Cancellation);
        await enter.HandleAsync(token, other, Cancellation);
        await scope.ServiceProvider.GetRequiredService<BlockUserHandler>().HandleAsync(album, owner, blocked, Cancellation);

        await using var delivery = database.CreateScope();
        var policy = delivery.ServiceProvider.GetRequiredService<ForgetDeletedAccount>();
        foreach (var deleted in new[] { person, person, blocked })
        {
            await policy.HandleAsync(new AccountDeleted(Guid.CreateVersion7(), DateTimeOffset.UtcNow, deleted, 2), Cancellation);
        }

        await using var check = database.CreateScope();
        Assert.Equal(other, Assert.Single(await check.ServiceProvider.GetRequiredService<IAudienceQueries>().ViewersAsync(album, Cancellation)).AccountId);
        Assert.False(await check.ServiceProvider.GetRequiredService<IBlockList>().IsBlockedAsync(album, blocked, Cancellation));
    }
}
