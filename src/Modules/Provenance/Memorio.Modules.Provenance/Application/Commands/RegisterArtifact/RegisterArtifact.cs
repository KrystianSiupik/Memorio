using Memorio.Modules.Provenance.Domain.Artifacts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Provenance.Application.Commands.RegisterArtifact;

/// <summary>C154: a module made something from people's content and records what from. The id comes from that module.</summary>
internal sealed record RegisterArtifact(Guid Id, string? Kind, Guid OwnerAccountId, Guid? AlbumId, IReadOnlyCollection<ArtifactSource> Sources);

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
            return Result.Success();
        }

        // Registered at the same moment by a retry of the same call.
        return await artifacts.FindAsync(command.Id, cancellationToken) is { } raced && raced.OwnerAccountId == command.OwnerAccountId
            ? Result.Success()
            : ArtifactErrors.IdTaken;
    }
}
