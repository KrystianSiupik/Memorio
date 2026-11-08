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
}
