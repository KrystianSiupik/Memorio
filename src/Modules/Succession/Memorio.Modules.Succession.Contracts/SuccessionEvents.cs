using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Succession.Contracts;

/// <summary>E121: the service checked the certificate; this owner is dead. Everything the person left follows from this.</summary>
public sealed record DeathConfirmedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid OwnerAccountId, int Sequence) : IIntegrationEvent;

/// <summary>E122: the report was not confirmed; whoever made it hears it.</summary>
public sealed record DeathReportRejectedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid CaseId, Guid AlbumId, Guid ReportedByAccountId, int Sequence) : IIntegrationEvent;

/// <summary>The successor can take the album over until <paramref name="AnswerBy"/>.</summary>
public sealed record TakeoverOfferedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid CaseId, Guid AlbumId, Guid SuccessorAccountId, DateTimeOffset AnswerBy, int Sequence) : IIntegrationEvent;

/// <summary>E123: the successor agreed.</summary>
public sealed record AlbumTransferredEvent(Guid EventId, DateTimeOffset OccurredAt, Guid CaseId, Guid AlbumId, Guid FromAccountId, Guid ToAccountId, int Sequence) : IIntegrationEvent;

/// <summary>E124: nobody takes it: a refusal, no answer in time, or no successor; only the effect.</summary>
public sealed record TakeoverRefusedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid CaseId, Guid AlbumId, Guid OwnerAccountId, int Sequence) : IIntegrationEvent;

/// <summary>E126: the contact named in advance may read what the owner kept private (S2:C8). The address comes from <see cref="ILegacyContacts"/>.</summary>
public sealed record PrivateArchiveSharedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid OwnerAccountId, int Sequence) : IIntegrationEvent;
