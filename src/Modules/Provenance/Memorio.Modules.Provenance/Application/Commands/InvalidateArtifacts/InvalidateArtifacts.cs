using Memorio.Modules.Provenance.Domain.Artifacts;

namespace Memorio.Modules.Provenance.Application.Commands.InvalidateArtifacts;

/// <summary>
/// C31, called by the policies: the fact is recorded first, then every valid artifact it hits is invalidated (E61). A
/// registration running at the same time finds the fact when it checks after its own save (D-023), so the order of the
/// two never leaves a live artifact with a missing source.
/// </summary>
internal sealed class InvalidateArtifactsHandler(IArtifacts artifacts, TimeProvider clock)
{
    /// <returns>How many artifacts were invalidated now.</returns>
    public async Task<int> HandleAsync(UnavailableSource fact, CancellationToken cancellationToken)
    {
        await artifacts.AddFactAsync(fact, cancellationToken);

        var invalidated = 0;
        foreach (var id in await artifacts.FindValidHitByAsync(fact, cancellationToken))
        {
            // A concurrent change of the same artifact (another fact) wins; it is invalid either way.
            if (await artifacts.FindAsync(id, cancellationToken) is { } artifact
                && fact.Hits(artifact)
                && artifact.Invalidate(fact.Reason, clock.GetUtcNow())
                && await artifacts.SaveAsync(cancellationToken))
            {
                invalidated++;
            }
        }

        return invalidated;
    }
}
