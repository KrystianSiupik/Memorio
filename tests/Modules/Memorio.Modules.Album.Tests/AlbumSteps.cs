using Memorio.Modules.Album.Application.Commands.Contributions;
using Memorio.Modules.Album.Application.Commands.CreateAlbum;
using Memorio.Modules.Album.Application.Commands.Invitations;
using Memorio.Modules.Album.Application.Queries;
using Memorio.Modules.Album.Domain.Albums;
using Memorio.SharedKernel.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Album.Tests;

internal static class AlbumSteps
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    public static async Task<AlbumId> NewAlbumAsync(this AlbumDatabase database, Guid ownerAccountId)
    {
        var id = Guid.CreateVersion7();
        await using var scope = database.CreateScope();
        var created = await scope.ServiceProvider.GetRequiredService<CreateAlbumHandler>().HandleAsync(new CreateAlbum(id, ownerAccountId, "Rodzina"), Cancellation);
        Assert.True(created.IsSuccess);
        return new AlbumId(id);
    }

    public static async Task<Result> InviteAsync(this AlbumDatabase database, AlbumId album, Guid ownerAccountId, string email, Guid? invitationId = null)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<InviteToAlbumHandler>().HandleAsync(
            new InviteToAlbum(album, ownerAccountId, invitationId ?? Guid.CreateVersion7(), email), Cancellation);
    }

    public static async Task<Result<AlbumId>> AnswerAsync(this AlbumDatabase database, Guid invitationId, Guid accountId, bool accepted)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<AnswerInvitationHandler>().HandleAsync(new AnswerInvitation(invitationId, accountId, accepted), Cancellation);
    }

    /// <summary>An album with its owner and one person who joined through an invitation.</summary>
    public static async Task<(AlbumId Album, Guid Owner, Guid Participant)> AlbumWithParticipantAsync(this AlbumDatabase database)
    {
        var owner = database.Neighbours.NewAccount();
        var participant = database.Neighbours.NewAccount();
        var album = await database.NewAlbumAsync(owner);
        var invitation = Guid.CreateVersion7();
        Assert.True((await database.InviteAsync(album, owner, database.Neighbours.EmailOf(participant), invitation)).IsSuccess);
        Assert.True((await database.AnswerAsync(invitation, participant, accepted: true)).IsSuccess);
        return (album, owner, participant);
    }

    public static async Task<Guid> BringMemoryAsync(this AlbumDatabase database, AlbumId album, Guid authorAccountId)
    {
        var memory = database.Neighbours.NewMemory(authorAccountId);
        Assert.True((await database.AddMemoryAsync(album, authorAccountId, memory)).IsSuccess);
        return memory;
    }

    public static async Task<Result> AddMemoryAsync(this AlbumDatabase database, AlbumId album, Guid accountId, Guid memoryId)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<AddMemoryToAlbumHandler>().HandleAsync(new AddMemoryToAlbum(album, accountId, memoryId), Cancellation);
    }

    public static async Task<AlbumView> ViewAsync(this AlbumDatabase database, AlbumId album)
    {
        await using var scope = database.CreateScope();
        return (await scope.ServiceProvider.GetRequiredService<IAlbumQueries>().ViewAsync(album.Value, Cancellation))!;
    }

    public static async Task<(int Participants, IReadOnlyList<InvitedRow> Invited)> SeatsAsync(this AlbumDatabase database, AlbumId album, Guid ownerAccountId)
    {
        await using var scope = database.CreateScope();
        return (await scope.ServiceProvider.GetRequiredService<IAlbumQueries>().SeatsAsync(album.Value, ownerAccountId, Cancellation))!.Value;
    }
}
