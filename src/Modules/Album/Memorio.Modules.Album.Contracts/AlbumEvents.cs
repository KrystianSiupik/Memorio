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

/// <summary>E162: the album has no owner. Only the effect, whatever the cause (D-001).</summary>
public sealed record AlbumWithoutOwnerEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, int Sequence) : IIntegrationEvent;

/// <summary>D-307: the living owner offers the album to this person, until <paramref name="AnswerBy"/>.</summary>
public sealed record HandoverOfferedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid ToAccountId, DateTimeOffset AnswerBy, int Sequence) : IIntegrationEvent;

/// <summary>The successor took the album over, or the person the living owner handed it to (D-307).</summary>
public sealed record AlbumOwnerChangedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid OwnerAccountId, int Sequence) : IIntegrationEvent;

/// <summary>E165: someone holds the album until this time.</summary>
public sealed record TakeoverReservedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, DateTimeOffset ReservedUntil, int Sequence) : IIntegrationEvent;

/// <summary>E167: a participant paid and is the owner now (D-004).</summary>
public sealed record AlbumTakenOverEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid OwnerAccountId, int Sequence) : IIntegrationEvent;

/// <summary>P34: a memory left the album on trimming (D-290: its author keeps it); what was made from it through the album goes.</summary>
public sealed record AlbumMemoryTrimmedEvent(Guid EventId, DateTimeOffset OccurredAt, Guid AlbumId, Guid MemoryId, Guid AuthorAccountId) : IIntegrationEvent;
