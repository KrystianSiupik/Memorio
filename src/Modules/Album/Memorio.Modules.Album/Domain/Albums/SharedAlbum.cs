using Memorio.SharedKernel.Domain;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Album.Domain.Albums;

/// <summary>
/// AG1: who is in a collection people build together. Every change of who is in it grows <see cref="Sequence"/>,
/// the concurrency token, so two parallel decisions never both count the same free seat (S2:B1, D-247).
/// </summary>
internal sealed class SharedAlbum : AggregateRoot<AlbumId>
{
    public const int MaximumNameLength = 120;

    private readonly List<AlbumParticipant> _participants = [];
    private readonly List<AlbumInvitation> _invitations = [];

    private SharedAlbum(AlbumId id, Guid ownerAccountId, Guid? successorAccountId)
        : base(id)
    {
        OwnerAccountId = ownerAccountId;
        SuccessorAccountId = successorAccountId;
    }

    public Guid OwnerAccountId { get; private set; }

    public Guid? SuccessorAccountId { get; private set; }

    public string Name { get; private set; } = "";

    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>Grows with every change of who is in the album; events carry it (D-013).</summary>
    public int Sequence { get; private set; } = 1;

    public IReadOnlyCollection<AlbumParticipant> Participants => _participants.AsReadOnly();

    public IReadOnlyCollection<AlbumInvitation> Invitations => _invitations.AsReadOnly();

    public static SharedAlbum Create(Guid ownerAccountId, Guid? successorAccountId = null) =>
        new(new AlbumId(Guid.CreateVersion7()), ownerAccountId, successorAccountId);

    /// <summary>C40. The id comes from the client (D-118).</summary>
    public static Result<SharedAlbum> Start(AlbumId id, Guid ownerAccountId, string? name, DateTimeOffset now)
    {
        var trimmed = name?.Trim();
        if (string.IsNullOrEmpty(trimmed) || trimmed.Length > MaximumNameLength)
        {
            return AlbumErrors.InvalidName;
        }

        var album = new SharedAlbum(id, ownerAccountId, null) { Name = trimmed, CreatedAt = now };
        album.Raise(new AlbumCreated(Guid.CreateVersion7(), now, id, ownerAccountId));
        return album;
    }

    public void AddParticipant(Guid accountId) => _participants.Add(new AlbumParticipant(accountId));

    public bool IsOwner(Guid accountId) => OwnerAccountId == accountId;

    public bool HasMember(Guid accountId) => IsOwner(accountId) || _participants.Exists(participant => participant.AccountId == accountId);

    public AlbumInvitation? FindInvitation(Guid invitationId) => _invitations.Find(invitation => invitation.Id == invitationId);

    /// <summary>The owner, everyone who joined and everyone invited who has not answered yet.</summary>
    public int SeatsTaken =>
        1 + _participants.Count(participant => participant.AccountId != OwnerAccountId)
        + _invitations.Count(invitation => invitation.State == InvitationState.Pending);

    /// <summary>C42. The same address again refreshes its invitation and takes no second seat.</summary>
    public Result<AlbumInvitation> Invite(Guid invitationId, Guid byAccountId, string email, int seatLimit, DateTimeOffset now)
    {
        if (!IsOwner(byAccountId))
        {
            return AlbumErrors.NoAccess;
        }

        var existing = _invitations.Find(invitation => invitation.State == InvitationState.Pending && invitation.Email == email);
        if (existing is not null)
        {
            existing.Refresh(now);
            Sequence++;
            Raise(new InvitationSent(Guid.CreateVersion7(), now, Id, existing.Id, Sequence));
            return existing;
        }

        if (SeatsTaken >= seatLimit)
        {
            return AlbumErrors.NoFreeSeats;
        }

        var invitation = AlbumInvitation.Create(invitationId, email, byAccountId, now);
        _invitations.Add(invitation);
        Sequence++;
        Raise(new InvitationSent(Guid.CreateVersion7(), now, Id, invitation.Id, Sequence));
        return invitation;
    }

    /// <summary>C44: the seat of the invitation becomes the seat of the participant.</summary>
    public Result Accept(Guid invitationId, Guid accountId, DateTimeOffset now)
    {
        var invitation = FindInvitation(invitationId);
        if (invitation?.State != InvitationState.Pending)
        {
            return AlbumErrors.InvitationGone;
        }

        invitation.Answer(InvitationState.Accepted);
        if (!HasMember(accountId))
        {
            _participants.Add(new AlbumParticipant(accountId, now));
        }

        Sequence++;
        Raise(new InvitationAccepted(Guid.CreateVersion7(), now, Id, accountId, Sequence));
        return Result.Success();
    }

    /// <summary>C45: the seat is free at once.</summary>
    public Result Decline(Guid invitationId, DateTimeOffset now)
    {
        var invitation = FindInvitation(invitationId);
        if (invitation?.State != InvitationState.Pending)
        {
            return AlbumErrors.InvitationGone;
        }

        invitation.Answer(InvitationState.Declined);
        Sequence++;
        Raise(new InvitationDeclined(Guid.CreateVersion7(), now, Id, invitationId, Sequence));
        return Result.Success();
    }

    /// <summary>The owner takes an invitation back: the seat is free at once.</summary>
    public Result Withdraw(Guid invitationId, Guid byAccountId, DateTimeOffset now)
    {
        if (!IsOwner(byAccountId))
        {
            return AlbumErrors.NoAccess;
        }

        var invitation = FindInvitation(invitationId);
        if (invitation?.State != InvitationState.Pending)
        {
            return AlbumErrors.InvitationGone;
        }

        invitation.Answer(InvitationState.Withdrawn);
        Sequence++;
        Raise(new InvitationWithdrawn(Guid.CreateVersion7(), now, Id, invitationId, Sequence));
        return Result.Success();
    }

    /// <summary>E45: only for an invitation still unanswered, and only once.</summary>
    public void MarkWeekPassed(Guid invitationId, DateTimeOffset now)
    {
        var invitation = FindInvitation(invitationId);
        if (invitation?.State != InvitationState.Pending || invitation.ReminderDueAt is not null)
        {
            return;
        }

        invitation.MarkReminderDue(now);
        Sequence++;
        Raise(new InvitationWeekPassed(Guid.CreateVersion7(), now, Id, invitationId, Sequence));
    }

    /// <summary>C52. The owner cannot leave: the album goes on through a successor (E9).</summary>
    public Result Leave(Guid accountId, DateTimeOffset now)
    {
        if (IsOwner(accountId))
        {
            return AlbumErrors.OwnerCannotLeave;
        }

        if (_participants.RemoveAll(participant => participant.AccountId == accountId) == 0)
        {
            return AlbumErrors.NoAccess;
        }

        ForgetSuccessor(accountId);
        Sequence++;
        Raise(new ParticipantLeft(Guid.CreateVersion7(), now, Id, accountId, Sequence));
        return Result.Success();
    }

    /// <summary>C53.</summary>
    public Result RemoveParticipant(Guid byAccountId, Guid accountId, DateTimeOffset now)
    {
        if (!IsOwner(byAccountId))
        {
            return AlbumErrors.NoAccess;
        }

        if (IsOwner(accountId))
        {
            return AlbumErrors.OwnerCannotLeave;
        }

        if (_participants.RemoveAll(participant => participant.AccountId == accountId) == 0)
        {
            return AlbumErrors.AlreadyDecided;
        }

        ForgetSuccessor(accountId);
        Sequence++;
        Raise(new ParticipantRemoved(Guid.CreateVersion7(), now, Id, accountId, Sequence));
        return Result.Success();
    }

    /// <summary>C41: only someone in the album can take it over.</summary>
    public Result NameSuccessor(Guid byAccountId, Guid accountId, DateTimeOffset now)
    {
        if (!IsOwner(byAccountId))
        {
            return AlbumErrors.NoAccess;
        }

        if (IsOwner(accountId) || !HasMember(accountId))
        {
            return AlbumErrors.SuccessorNotParticipant;
        }

        SuccessorAccountId = accountId;
        Sequence++;
        Raise(new SuccessorNamed(Guid.CreateVersion7(), now, Id, accountId, Sequence));
        return Result.Success();
    }

    public void RecordMemoryAdded(Guid memoryId, Guid authorAccountId, DateTimeOffset now) =>
        Raise(new MemoryAddedToAlbum(Guid.CreateVersion7(), now, Id, memoryId, authorAccountId));

    public void RecordPhotoAdded(Guid memoryId, Guid photoId, DateTimeOffset now) =>
        Raise(new PhotoAddedInAlbum(Guid.CreateVersion7(), now, Id, memoryId, photoId));

    /// <summary>E51: nothing changes in the album, only the owner gets to know.</summary>
    public void RecordPoolExhausted(DateTimeOffset now) =>
        Raise(new AlbumPoolExhausted(Guid.CreateVersion7(), now, Id, OwnerAccountId, Sequence));

    public void RecordContributionsWithdrawn(Guid authorAccountId, DateTimeOffset now)
    {
        Sequence++;
        Raise(new ContributionsWithdrawn(Guid.CreateVersion7(), now, Id, authorAccountId, Sequence));
    }

    public void RecordContributionsKept(Guid authorAccountId, DateTimeOffset now) =>
        Raise(new ContributionsKept(Guid.CreateVersion7(), now, Id, authorAccountId));

    public void RecordEntrusted(Guid authorAccountId, Guid curatorAccountId, DateTimeOffset now) =>
        Raise(new ContentEntrusted(Guid.CreateVersion7(), now, Id, authorAccountId, curatorAccountId));

    public void RecordEntrustmentRevoked(Guid authorAccountId, DateTimeOffset now) =>
        Raise(new EntrustmentRevoked(Guid.CreateVersion7(), now, Id, authorAccountId));

    public void RecordArranged(Guid curatorAccountId, DateTimeOffset now) =>
        Raise(new AlbumArranged(Guid.CreateVersion7(), now, Id, curatorAccountId));

    public void RecordTagged(Guid memoryId, Guid accountId, Guid byAccountId, DateTimeOffset now) =>
        Raise(new ParticipantTagged(Guid.CreateVersion7(), now, Id, memoryId, accountId, byAccountId, Sequence));

    public void RecordTagRemoved(Guid memoryId, Guid accountId, DateTimeOffset now) =>
        Raise(new TagRemoved(Guid.CreateVersion7(), now, Id, memoryId, accountId));

    private void ForgetSuccessor(Guid accountId)
    {
        if (SuccessorAccountId == accountId)
        {
            SuccessorAccountId = null;
        }
    }
}

internal sealed record AlbumParticipant(Guid AccountId, DateTimeOffset JoinedAt = default);
