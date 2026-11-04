using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Album.Contracts;

/// <summary>E42: the invitation waits for its answer.</summary>
public sealed record InvitationSentEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid InvitationId, int Sequence) : IIntegrationEvent;

/// <summary>E45: 7 days without an answer.</summary>
public sealed record InvitationWeekPassedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid InvitationId, int Sequence) : IIntegrationEvent;

/// <summary>E47: the person is in the album.</summary>
public sealed record InvitationAcceptedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid AccountId, int Sequence) : IIntegrationEvent;

/// <summary>E51: something did not fit in the album's pool.</summary>
public sealed record AlbumPoolExhaustedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid OwnerAccountId, int Sequence) : IIntegrationEvent;

/// <summary>E56: the seat is free.</summary>
public sealed record ParticipantLeftEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid AccountId, int Sequence) : IIntegrationEvent;

/// <summary>E58: what this author brought is no longer in the album.</summary>
public sealed record ContributionsWithdrawnEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid AccountId, int Sequence) : IIntegrationEvent;

/// <summary>E59: the owner removed the person; the seat is free.</summary>
public sealed record ParticipantRemovedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid AccountId, int Sequence) : IIntegrationEvent;

/// <summary>E175: the person was tagged in a memory of the album.</summary>
public sealed record ParticipantTaggedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid MemoryId, Guid AccountId, int Sequence) : IIntegrationEvent;
