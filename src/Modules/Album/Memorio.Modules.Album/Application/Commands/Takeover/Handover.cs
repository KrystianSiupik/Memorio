using Memorio.Modules.Album.Application.Commands.Contributions;
using Memorio.Modules.Album.Domain.Albums;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Album.Application.Commands.Takeover;

/// <summary>D-307: the living owner hands the album to someone in it (S2:B3), choosing now what becomes of their own contributions.</summary>
internal sealed record OfferHandover(AlbumId AlbumId, Guid AccountId, Guid ToAccountId, bool KeepContributions);

internal sealed record WithdrawHandover(AlbumId AlbumId, Guid AccountId);

internal sealed record AnswerHandover(AlbumId AlbumId, Guid AccountId, bool Accepted);

internal sealed class HandoverHandler(IAlbums albums, ContributionWithdrawal withdrawal, TimeProvider clock)
{
    public Task<Result> HandleAsync(OfferHandover command, CancellationToken cancellationToken) =>
        DecideAsync(command.AlbumId, album => Task.FromResult(album.OfferHandover(command.AccountId, command.ToAccountId, command.KeepContributions, clock.GetUtcNow())), cancellationToken);

    public Task<Result> HandleAsync(WithdrawHandover command, CancellationToken cancellationToken) =>
        DecideAsync(command.AlbumId, album => Task.FromResult(album.WithdrawHandover(command.AccountId)), cancellationToken);

    /// <summary>Accepting takes the former owner's contributions out in the same save when they chose so (C30).</summary>
    public Task<Result> HandleAsync(AnswerHandover command, CancellationToken cancellationToken) =>
        DecideAsync(
            command.AlbumId,
            async album =>
            {
                if (!command.Accepted)
                {
                    return album.RefuseHandover(command.AccountId);
                }

                var now = clock.GetUtcNow();
                var accepted = album.AcceptHandover(command.AccountId, now);
                if (accepted.IsFailure)
                {
                    return accepted.Error;
                }

                if (!accepted.Value.KeepContributions)
                {
                    await withdrawal.WithdrawAsync(album, accepted.Value.FormerOwner, now, cancellationToken);
                }

                return Result.Success();
            },
            cancellationToken);

    private Task<Result> DecideAsync(AlbumId albumId, Func<SharedAlbum, Task<Result>> decide, CancellationToken cancellationToken) =>
        AlbumDecision.UntilStoredAsync(
            albums,
            async () => await albums.FindAsync(albumId, cancellationToken) is { } album ? await decide(album) : AlbumErrors.NoAccess,
            cancellationToken);
}
