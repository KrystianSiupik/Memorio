namespace Memorio.Modules.Provenance.Domain.Artifacts;

/// <summary>The artifacts and their sources; a save publishes what they raised through the outbox.</summary>
internal interface IArtifacts
{
    Task<Artifact?> FindAsync(Guid id, CancellationToken cancellationToken);

    void Add(Artifact artifact);

    /// <summary>Records the fact once; the same fact (event) again adds nothing.</summary>
    Task AddFactAsync(UnavailableSource fact, CancellationToken cancellationToken);

    /// <summary>Valid artifacts made from what the fact takes away.</summary>
    Task<IReadOnlyList<Guid>> FindValidHitByAsync(UnavailableSource fact, CancellationToken cancellationToken);

    /// <summary>Facts from this moment on (D-023: the check after a registration).</summary>
    Task<IReadOnlyList<UnavailableSource>> FindFactsSinceAsync(DateTimeOffset since, CancellationToken cancellationToken);

    /// <summary>False when somebody else saved first (a concurrent change or the same id).</summary>
    Task<bool> SaveAsync(CancellationToken cancellationToken);
}
