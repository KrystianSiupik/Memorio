namespace Memorio.Modules.AiNarration.Domain.Consents;

/// <summary>The consents; a save publishes what they raised through the outbox.</summary>
internal interface IAiConsents
{
    Task<AiConsent?> FindAsync(Guid accountId, Guid albumId, CancellationToken cancellationToken);

    void Add(AiConsent consent);

    /// <summary>False when somebody else saved first (the same person and album at once).</summary>
    Task<bool> SaveAsync(CancellationToken cancellationToken);
}
