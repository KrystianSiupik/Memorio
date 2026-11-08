using Memorio.Modules.Provenance.Application.Commands.RegisterArtifact;
using Memorio.Modules.Provenance.Contracts;
using Memorio.Modules.Provenance.Domain.Artifacts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Provenance.Infrastructure;

/// <summary>The write port (C154) over the module's own command.</summary>
internal sealed class ArtifactRegistry(RegisterArtifactHandler handler) : IArtifactRegistry
{
    public Task<Result> RegisterAsync(NewArtifact artifact, CancellationToken cancellationToken) =>
        handler.HandleAsync(
            new RegisterArtifact(
                artifact.Id,
                artifact.Kind,
                artifact.OwnerAccountId,
                artifact.AlbumId,
                [.. artifact.Sources.Select(source => new ArtifactSource(source.MemoryId, source.AuthorAccountId, source.AlbumId))])
            {
                MaterialsReadAt = artifact.MaterialsReadAt,
            },
            cancellationToken);
}
