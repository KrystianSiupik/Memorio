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

    /// <summary>The owner of an album without one: no account has this id, so nobody is its owner (D-293).</summary>
    public static readonly Guid NoOwner = Guid.Empty;

    /// <summary>What a participant has to pay in (D-004).</summary>
    public static readonly TimeSpan ReservationTime = TimeSpan.FromMinutes(30);

    /// <summary>
    /// How long after the 30 minutes a payment still counts for whoever held the reservation, unless someone else reserved
    /// since: a store purchase started in time may arrive late (D-306).
    /// </summary>
    public static readonly TimeSpan LatePaymentGrace = TimeSpan.FromHours(24);

    /// <summary>How long a handover while alive waits for the answer, as an offer after a death (HS-021, D-307).</summary>
    public static readonly TimeSpan HandoverWindow = TimeSpan.FromDays(30);

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

    /// <summary>When the album lost its owner; set together with <see cref="NoOwner"/> as the owner (D-001).</summary>
    public DateTimeOffset? OwnerlessSince { get; private set; }

    public Guid? ReservedByAccountId { get; private set; }

    public DateTimeOffset? ReservedUntil { get; private set; }

    /// <summary>C-HO1: who the living owner offers the album to; one at a time (D-307).</summary>
    public Guid? HandoverToAccountId { get; private set; }

    /// <summary>The owner's choice for their own contributions, made when offering (S2:B3): true keeps them in the album.</summary>
    public bool? HandoverKeepsContributions { get; private set; }

    public DateTimeOffset? HandoverUntil { get; private set; }

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

    public bool IsOwnerless => OwnerAccountId == NoOwner;

    public bool HasActiveReservation(DateTimeOffset now) => ReservedUntil > now;

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
        ForgetHandover(accountId);
        if (ReservedByAccountId == accountId)
        {
            ReservedByAccountId = null;
            ReservedUntil = null;
        }

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
        ForgetHandover(accountId);
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

    /// <summary>C153 and E162: only the effect, whatever the cause was (D-001). Asking again changes nothing.</summary>
    public void ReleaseOwner(DateTimeOffset now)
    {
        if (IsOwnerless)
        {
            return;
        }

        OwnerAccountId = NoOwner;
        SuccessorAccountId = null;
        ForgetHandover(HandoverToAccountId);
        OwnerlessSince = now;
        Sequence++;
        Raise(new AlbumWithoutOwner(Guid.CreateVersion7(), now, Id, Sequence));
    }

    /// <summary>
    /// C122: the album, its pool and the role go to the successor (S2:B3). The owner's contributions stay in it. A successor
    /// who left in the meantime cannot take it: then it is as after a refusal. Already done, or decided otherwise: nothing.
    /// </summary>
    public void TransferOwnership(Guid fromAccountId, Guid toAccountId, DateTimeOffset now)
    {
        if (!IsOwner(fromAccountId))
        {
            return;
        }

        if (!_participants.Exists(participant => participant.AccountId == toAccountId))
        {
            ReleaseOwner(now);
            return;
        }

        BecomeOwner(toAccountId);
        Sequence++;
        Raise(new AlbumOwnerChanged(Guid.CreateVersion7(), now, Id, toAccountId, Sequence));
    }

    /// <summary>C155: one reservation at a time, first come first served; asking again keeps one's own.</summary>
    public Result ReserveTakeover(Guid accountId, DateTimeOffset now)
    {
        if (!IsOwnerless)
        {
            return AlbumErrors.HasOwner;
        }

        if (!_participants.Exists(participant => participant.AccountId == accountId))
        {
            return AlbumErrors.NoAccess;
        }

        if (HasActiveReservation(now))
        {
            return ReservedByAccountId == accountId ? Result.Success() : AlbumErrors.TakeoverReserved;
        }

        ReservedByAccountId = accountId;
        ReservedUntil = now + ReservationTime;
        Sequence++;
        Raise(new TakeoverReserved(Guid.CreateVersion7(), now, Id, accountId, ReservedUntil.Value, Sequence));
        return Result.Success();
    }

    /// <summary>
    /// C157: the 30 minutes alone let someone else reserve; who held it is forgotten only once a late payment can no longer
    /// count (D-306).
    /// </summary>
    public void ExpireReservation(DateTimeOffset now)
    {
        if (ReservedUntil is null || now < ReservedUntil + LatePaymentGrace)
        {
            return;
        }

        ReservedByAccountId = null;
        ReservedUntil = null;
        Sequence++;
    }

    /// <summary>
    /// C156: whoever holds the reservation and paid, also late, up to <see cref="LatePaymentGrace"/> after its time, when
    /// nobody reserved since (D-306). Otherwise E168: nothing changes.
    /// </summary>
    public Result TakeOver(DateTimeOffset now)
    {
        if (!IsOwnerless || ReservedByAccountId is null || !(now < ReservedUntil + LatePaymentGrace))
        {
            return AlbumErrors.TakeoverExpired;
        }

        var newOwner = ReservedByAccountId!.Value;
        BecomeOwner(newOwner);
        Sequence++;
        Raise(new AlbumTakenOver(Guid.CreateVersion7(), now, Id, newOwner, Sequence));
        return Result.Success();
    }

    /// <summary>
    /// D-307: the living owner offers the album to someone in it, and says now what becomes of their own contributions
    /// (S2:B3). A new offer replaces the earlier one. Nothing changes until the person accepts.
    /// </summary>
    public Result OfferHandover(Guid byAccountId, Guid toAccountId, bool keepContributions, DateTimeOffset now)
    {
        if (!IsOwner(byAccountId))
        {
            return AlbumErrors.NoAccess;
        }

        if (!_participants.Exists(participant => participant.AccountId == toAccountId))
        {
            return AlbumErrors.HandoverNotParticipant;
        }

        if (HandoverToAccountId == toAccountId && HandoverKeepsContributions == keepContributions && HandoverUntil > now)
        {
            return Result.Success();
        }

        HandoverToAccountId = toAccountId;
        HandoverKeepsContributions = keepContributions;
        HandoverUntil = now + HandoverWindow;
        Sequence++;
        Raise(new HandoverOffered(Guid.CreateVersion7(), now, Id, toAccountId, HandoverUntil.Value, Sequence));
        return Result.Success();
    }

    /// <summary>The owner takes the offer back; asking again changes nothing.</summary>
    public Result WithdrawHandover(Guid byAccountId)
    {
        if (!IsOwner(byAccountId))
        {
            return AlbumErrors.NoAccess;
        }

        if (HandoverToAccountId is not null)
        {
            ForgetHandover(HandoverToAccountId);
            Sequence++;
        }

        return Result.Success();
    }

    /// <summary>
    /// D-307: the album, its pool and the role go to the person; the former owner stays in it as a participant. Gives back
    /// whether the former owner's contributions stay; taking them is the caller's step, through C30.
    /// </summary>
    public Result<(Guid FormerOwner, bool KeepContributions)> AcceptHandover(Guid accountId, DateTimeOffset now)
    {
        if (HandoverToAccountId != accountId || !(HandoverUntil > now) || IsOwnerless)
        {
            return AlbumErrors.HandoverGone;
        }

        var former = OwnerAccountId;
        var keep = HandoverKeepsContributions ?? true;
        BecomeOwner(accountId);
        _participants.Add(new AlbumParticipant(former, now));
        ForgetHandover(accountId);
        Sequence++;
        Raise(new AlbumOwnerChanged(Guid.CreateVersion7(), now, Id, accountId, Sequence));
        return (former, keep);
    }

    /// <summary>The person refuses: the album stays with its owner.</summary>
    public Result RefuseHandover(Guid accountId)
    {
        if (HandoverToAccountId != accountId)
        {
            return AlbumErrors.HandoverGone;
        }

        ForgetHandover(accountId);
        Sequence++;
        return Result.Success();
    }

    private void ForgetHandover(Guid? accountId)
    {
        if (accountId is not null && HandoverToAccountId == accountId)
        {
            HandoverToAccountId = null;
            HandoverKeepsContributions = null;
            HandoverUntil = null;
        }
    }

    private void BecomeOwner(Guid accountId)
    {
        _participants.RemoveAll(participant => participant.AccountId == accountId);
        OwnerAccountId = accountId;
        SuccessorAccountId = null;
        OwnerlessSince = null;
        ReservedByAccountId = null;
        ReservedUntil = null;
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
