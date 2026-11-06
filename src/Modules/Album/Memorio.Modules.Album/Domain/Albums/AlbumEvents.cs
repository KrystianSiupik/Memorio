using Memorio.SharedKernel.Domain;

namespace Memorio.Modules.Album.Domain.Albums;

/// <summary>E40.</summary>
internal sealed record AlbumCreated(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid OwnerAccountId) : IDomainEvent;

/// <summary>E41.</summary>
internal sealed record SuccessorNamed(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid AccountId, int Sequence) : IDomainEvent;

/// <summary>E42.</summary>
internal sealed record InvitationSent(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid InvitationId, int Sequence) : IDomainEvent;

/// <summary>E45.</summary>
internal sealed record InvitationWeekPassed(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid InvitationId, int Sequence) : IDomainEvent;

/// <summary>E47.</summary>
internal sealed record InvitationAccepted(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid AccountId, int Sequence) : IDomainEvent;

/// <summary>E48.</summary>
internal sealed record InvitationDeclined(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid InvitationId, int Sequence) : IDomainEvent;

internal sealed record InvitationWithdrawn(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid InvitationId, int Sequence) : IDomainEvent;

/// <summary>E50.</summary>
internal sealed record MemoryAddedToAlbum(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid MemoryId, Guid AuthorAccountId) : IDomainEvent;

/// <summary>E51.</summary>
internal sealed record AlbumPoolExhausted(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid OwnerAccountId, int Sequence) : IDomainEvent;

/// <summary>E53.</summary>
internal sealed record ContentEntrusted(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid AuthorAccountId, Guid CuratorAccountId) : IDomainEvent;

/// <summary>E54.</summary>
internal sealed record EntrustmentRevoked(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid AuthorAccountId) : IDomainEvent;

/// <summary>E55.</summary>
internal sealed record AlbumArranged(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid CuratorAccountId) : IDomainEvent;

/// <summary>E56.</summary>
internal sealed record ParticipantLeft(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid AccountId, int Sequence) : IDomainEvent;

/// <summary>E57.</summary>
internal sealed record ContributionsKept(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid AuthorAccountId) : IDomainEvent;

/// <summary>E58.</summary>
internal sealed record ContributionsWithdrawn(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid AuthorAccountId, int Sequence) : IDomainEvent;

/// <summary>E59.</summary>
internal sealed record ParticipantRemoved(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid AccountId, int Sequence) : IDomainEvent;

/// <summary>E69.</summary>
internal sealed record PhotoAddedInAlbum(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid MemoryId, Guid PhotoId) : IDomainEvent;

/// <summary>E162.</summary>
internal sealed record AlbumWithoutOwner(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, int Sequence) : IDomainEvent;

/// <summary>The successor is the owner now (after E123), or the person the living owner handed it to (D-307).</summary>
internal sealed record AlbumOwnerChanged(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid OwnerAccountId, int Sequence) : IDomainEvent;

/// <summary>D-307: the living owner offers the album to someone in it.</summary>
internal sealed record HandoverOffered(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid ToAccountId, DateTimeOffset AnswerBy, int Sequence) : IDomainEvent;

/// <summary>E165.</summary>
internal sealed record TakeoverReserved(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid AccountId, DateTimeOffset ReservedUntil, int Sequence) : IDomainEvent;

/// <summary>E167.</summary>
internal sealed record AlbumTakenOver(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid OwnerAccountId, int Sequence) : IDomainEvent;

/// <summary>E175.</summary>
internal sealed record ParticipantTagged(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid MemoryId, Guid AccountId, Guid ByAccountId, int Sequence) : IDomainEvent;

/// <summary>E176.</summary>
internal sealed record TagRemoved(Guid EventId, DateTimeOffset OccurredAt, AlbumId AlbumId, Guid MemoryId, Guid AccountId) : IDomainEvent;
