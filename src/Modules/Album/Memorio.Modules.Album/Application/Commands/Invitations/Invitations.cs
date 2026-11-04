using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Album.Domain.Albums;
using Memorio.Modules.Notifications.Contracts;
using Memorio.Modules.Plans.Contracts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Album.Application.Commands.Invitations;

/// <summary>C42.</summary>
internal sealed record InviteToAlbum(AlbumId AlbumId, Guid AccountId, Guid InvitationId, string? Email);

internal sealed class InviteToAlbumHandler(
    IAlbums albums,
    IAlbumSeatLimit seats,
    IInvitationBlocks blocks,
    IAccountDirectory directory,
    TimeProvider clock)
{
    public async Task<Result> HandleAsync(InviteToAlbum command, CancellationToken cancellationToken)
    {
        var email = InvitationEmail.Normalize(command.Email);
        if (email is null)
        {
            return AlbumErrors.InvalidEmail;
        }

        if (await albums.FindAsync(command.AlbumId, cancellationToken) is not { } first || !first.IsOwner(command.AccountId))
        {
            return AlbumErrors.NoAccess;
        }

        if (InvitationEmail.Normalize(await directory.EmailOfAsync(command.AccountId, cancellationToken)) == email)
        {
            return AlbumErrors.CannotInviteYourself;
        }

        if (await blocks.IsBlockedAsync(email, cancellationToken))
        {
            return AlbumErrors.AddressBlocked;
        }

        var seatLimit = await seats.SeatsOfAsync(command.AlbumId.Value, cancellationToken);
        return await AlbumDecision.UntilStoredAsync(
            albums,
            async () =>
            {
                var album = await albums.FindAsync(command.AlbumId, cancellationToken);
                if (album is null)
                {
                    return AlbumErrors.NoAccess;
                }

                var invited = album.Invite(command.InvitationId, command.AccountId, email, seatLimit, clock.GetUtcNow());
                return invited.IsFailure ? invited.Error : Result.Success();
            },
            cancellationToken);
    }
}

/// <summary>C44 and C45: only the person the invitation was sent to answers it.</summary>
internal sealed record AnswerInvitation(Guid InvitationId, Guid AccountId, bool Accepted);

internal sealed class AnswerInvitationHandler(IAlbums albums, IAccountDirectory directory, TimeProvider clock)
{
    public async Task<Result<AlbumId>> HandleAsync(AnswerInvitation command, CancellationToken cancellationToken)
    {
        var email = InvitationEmail.Normalize(await directory.EmailOfAsync(command.AccountId, cancellationToken));
        AlbumId albumId = default;
        var decided = await AlbumDecision.UntilStoredAsync(
            albums,
            async () =>
            {
                var album = await albums.FindByInvitationAsync(command.InvitationId, cancellationToken);
                if (album is null || album.FindInvitation(command.InvitationId)?.Email != email)
                {
                    return AlbumErrors.InvitationGone;
                }

                albumId = album.Id;
                return command.Accepted
                    ? album.Accept(command.InvitationId, command.AccountId, clock.GetUtcNow())
                    : album.Decline(command.InvitationId, clock.GetUtcNow());
            },
            cancellationToken);
        return decided.IsFailure ? decided.Error : albumId;
    }
}

internal sealed record WithdrawInvitation(AlbumId AlbumId, Guid AccountId, Guid InvitationId);

internal sealed class WithdrawInvitationHandler(IAlbums albums, TimeProvider clock)
{
    public Task<Result> HandleAsync(WithdrawInvitation command, CancellationToken cancellationToken) =>
        AlbumDecision.UntilStoredAsync(
            albums,
            async () =>
            {
                var album = await albums.FindAsync(command.AlbumId, cancellationToken);
                return album is null
                    ? AlbumErrors.NoAccess
                    : album.Withdraw(command.InvitationId, command.AccountId, clock.GetUtcNow());
            },
            cancellationToken);
}
