namespace Memorio.Modules.Provenance.Domain.Artifacts;

/// <summary>The artifacts and their sources; a save publishes what they raised through the outbox.</summary>
internal interface IArtifacts
{
    Task<Artifact?> FindAsync(Guid id, CancellationToken cancellationToken);

    void Add(Artifact artifact);

    /// <summary>False when somebody else saved first (a concurrent change or the same id).</summary>
    Task<bool> SaveAsync(CancellationToken cancellationToken);
}
