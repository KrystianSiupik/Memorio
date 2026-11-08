using Memorio.Modules.AiNarration.Application.Commands.Consents;
using Memorio.Modules.AiNarration.Domain.Consents;
using Memorio.Modules.Album.Contracts;

namespace Memorio.Modules.AiNarration.Application.Policies;

/// <summary>D-335 (HS-004): the AI consent for an album lapses when the person leaves it or is removed from it.</summary>
internal sealed class ConsentEndsWithParticipation(RevokeAiConsentHandler revoke)
{
    public Task HandleAsync(ParticipantLeftEvent left, CancellationToken cancellationToken) =>
        revoke.HandleAsync(new RevokeAiConsent(left.AccountId, left.AlbumId, AiConsentEnd.LeftAlbum), cancellationToken);

    public Task HandleAsync(ParticipantRemovedEvent removed, CancellationToken cancellationToken) =>
        revoke.HandleAsync(new RevokeAiConsent(removed.AccountId, removed.AlbumId, AiConsentEnd.RemovedFromAlbum), cancellationToken);
}
