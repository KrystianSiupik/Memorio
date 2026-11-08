using Memorio.Modules.Provenance.Domain.Artifacts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Provenance.Application.Commands.RegisterArtifact;

/// <summary>C154: a module made something from people's content and records what from. The id comes from that module.</summary>
internal sealed record RegisterArtifact(Guid Id, string? Kind, Guid OwnerAccountId, Guid? AlbumId, IReadOnlyCollection<ArtifactSource> Sources)
{
    /// <summary>When the maker read the materials; what was withdrawn after it invalidates the artifact (C31, AG9).</summary>
    public DateTimeOffset? MaterialsReadAt { get; init; }
}

/// <summary>Registering the same artifact again changes nothing; another artifact's id is refused.</summary>
internal sealed class RegisterArtifactHandler(IArtifacts artifacts, TimeProvider clock)
{
    public async Task<Result> HandleAsync(RegisterArtifact command, CancellationToken cancellationToken)
    {
        if (await artifacts.FindAsync(command.Id, cancellationToken) is { } existing)
        {
            return existing.OwnerAccountId == command.OwnerAccountId && existing.Kind == command.Kind?.Trim()
                ? Result.Success()
                : ArtifactErrors.IdTaken;
        }

        var artifact = Artifact.Register(command.Id, command.Kind, command.OwnerAccountId, command.AlbumId, command.Sources, clock.GetUtcNow());
        if (artifact.IsFailure)
        {
            return artifact.Error;
        }

        artifacts.Add(artifact.Value);
        if (await artifacts.SaveAsync(cancellationToken))
        {
            await CheckAfterSaveAsync(artifact.Value, command.MaterialsReadAt, cancellationToken);
            return Result.Success();
        }

        // Registered at the same moment by a retry of the same call.
        return await artifacts.FindAsync(command.Id, cancellationToken) is { } raced && raced.OwnerAccountId == command.OwnerAccountId
            ? Result.Success()
            : ArtifactErrors.IdTaken;
    }

    /// <summary>
    /// AG9, D-023: saved first, then checked. What was withdrawn since the materials were read invalidates the new artifact
    /// at once; what is withdrawn after this check finds the artifact through the policy.
    /// </summary>
    private async Task CheckAfterSaveAsync(Artifact artifact, DateTimeOffset? materialsReadAt, CancellationToken cancellationToken)
    {
        if (materialsReadAt is not { } since)
        {
            return;
        }

        var fact = (await artifacts.FindFactsSinceAsync(since, cancellationToken)).FirstOrDefault(found => found.Hits(artifact));
        if (fact is not null && artifact.Invalidate(fact.Reason, clock.GetUtcNow()))
        {
            await artifacts.SaveAsync(cancellationToken);
        }
    }
}
