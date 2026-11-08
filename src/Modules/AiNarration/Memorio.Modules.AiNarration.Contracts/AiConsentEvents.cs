using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.AiNarration.Contracts;

/// <summary>E80: the person lets the AI use their content in this album (S2:C5).</summary>
public sealed record AiConsentGivenEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AccountId, Guid AlbumId, int Sequence) : IIntegrationEvent;

/// <summary>
/// E81: the person no longer lets the AI use their content in this album. Provenance invalidates what was made from it
/// there (P12); new summaries leave it out.
/// </summary>
/// <param name="Reason">ByPerson, LeftAlbum or RemovedFromAlbum (D-335: the consent lapses with the participation).</param>
public sealed record AiConsentRevokedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AccountId, Guid AlbumId, string Reason, int Sequence) : IIntegrationEvent;

/// <summary>The values of <see cref="AiConsentRevokedEvent.Reason"/>.</summary>
public static class AiConsentRevocationReasons
{
    public const string ByPerson = "ByPerson";

    public const string LeftAlbum = "LeftAlbum";

    public const string RemovedFromAlbum = "RemovedFromAlbum";
}
