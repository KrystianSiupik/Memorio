using Memorio.Modules.Access.Contracts;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Provenance.Application.Commands.InvalidateArtifacts;
using Memorio.Modules.Provenance.Domain.Artifacts;

namespace Memorio.Modules.Provenance.Application.Policies;

/// <summary>The cascade (S2:B2): when content may no longer be used, what was made from it goes too (C31).</summary>
internal sealed class InvalidateWhenSourceGone(InvalidateArtifactsHandler invalidate)
{
    /// <summary>P4: a memory deleted for good (E19), wherever it was cited.</summary>
    public Task HandleAsync(ContentDeleted deleted, CancellationToken cancellationToken) =>
        invalidate.HandleAsync(
            UnavailableSource.Memory(deleted.EventId, deleted.MemoryId, InvalidationReason.ContentDeleted, deleted.OccurredAt),
            cancellationToken);

    /// <summary>P9: an author took their contributions out of an album (E57); what was made from them through it goes.</summary>
    public Task HandleAsync(ContributionsWithdrawnEvent withdrawn, CancellationToken cancellationToken) =>
        invalidate.HandleAsync(
            UnavailableSource.AuthorInAlbum(withdrawn.EventId, withdrawn.AlbumId, withdrawn.AccountId, InvalidationReason.ContributionsWithdrawn, withdrawn.OccurredAt),
            cancellationToken);

    /// <summary>P10: a person lost the album (E66); everything made for them from it goes, and does not come back.</summary>
    public Task HandleAsync(AccessRevokedEvent revoked, CancellationToken cancellationToken) =>
        invalidate.HandleAsync(
            UnavailableSource.ReaderOfAlbum(revoked.EventId, revoked.AlbumId, revoked.AccountId, InvalidationReason.AccessRevoked, revoked.OccurredAt),
            cancellationToken);

    /// <summary>P10 too: blocking takes the album away (C64) the same way.</summary>
    public Task HandleAsync(UserBlockedEvent blocked, CancellationToken cancellationToken) =>
        invalidate.HandleAsync(
            UnavailableSource.ReaderOfAlbum(blocked.EventId, blocked.AlbumId, blocked.AccountId, InvalidationReason.AccessRevoked, blocked.OccurredAt),
            cancellationToken);
}
