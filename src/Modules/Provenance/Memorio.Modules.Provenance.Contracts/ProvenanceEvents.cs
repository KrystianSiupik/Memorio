using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Provenance.Contracts;

/// <summary>E61: an artifact may no longer exist; its maker hides it for good (S2:B2: it does not come back).</summary>
/// <param name="Reason">ContentDeleted, ContributionsWithdrawn, AccessRevoked, AiConsentRevoked or ArchiveTrimmed.</param>
public sealed record ArtifactInvalidatedEvent(
    Guid EventId,
    DateTimeOffset OccurredAt,
    Guid ArtifactId,
    string Kind,
    Guid OwnerAccountId,
    string Reason,
    int Sequence) : IIntegrationEvent;
