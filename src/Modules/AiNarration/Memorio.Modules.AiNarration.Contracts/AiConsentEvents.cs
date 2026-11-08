using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.AiNarration.Contracts;

/// <summary>E80: the person lets the AI use their content in this album (S2:C5).</summary>
public sealed record AiConsentGivenEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AccountId, Guid AlbumId, int Sequence) : IIntegrationEvent;
