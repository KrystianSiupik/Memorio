using Memorio.Modules.Album.Domain.Albums;

namespace Memorio.Modules.Album.Tests;

/// <summary>AG1 on its own: MEM-E3-ALBUM-01 to -04, -09, -10.</summary>
public sealed class SharedAlbumTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 4, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid Owner = Guid.CreateVersion7();

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public void An_album_needs_a_name(string? name) =>
        Assert.Equal(AlbumErrors.InvalidName, SharedAlbum.Start(new AlbumId(Guid.CreateVersion7()), Owner, name, Now).Error);

    [Fact]
    public void A_name_too_long_is_refused() =>
        Assert.Equal(AlbumErrors.InvalidName, SharedAlbum.Start(new AlbumId(Guid.CreateVersion7()), Owner, new string('a', SharedAlbum.MaximumNameLength + 1), Now).Error);

    [Fact]
    public void A_new_album_has_its_owner_and_gives_E40()
    {
        var album = NewAlbum();

        Assert.True(album.HasMember(Owner));
        Assert.Equal(1, album.SeatsTaken);
        Assert.IsType<AlbumCreated>(Assert.Single(album.DomainEvents));
    }

    [Fact]
    public void Participants_and_unanswered_invitations_never_exceed_the_seats()
    {
        var album = NewAlbum();

        Assert.True(album.Invite(Guid.CreateVersion7(), Owner, "a@example.invalid", seatLimit: 2, Now).IsSuccess);
        var refused = album.Invite(Guid.CreateVersion7(), Owner, "b@example.invalid", seatLimit: 2, Now);

        Assert.Equal(AlbumErrors.NoFreeSeats, refused.Error);
        Assert.Equal(2, album.SeatsTaken);
    }

    [Fact]
    public void Inviting_the_same_address_again_refreshes_it_and_takes_no_second_seat()
    {
        var album = NewAlbum();
        var first = album.Invite(Guid.CreateVersion7(), Owner, "a@example.invalid", seatLimit: 2, Now).Value;

        var second = album.Invite(Guid.CreateVersion7(), Owner, "a@example.invalid", seatLimit: 2, Now.AddDays(1)).Value;

        Assert.Equal(first.Id, second.Id);
        Assert.Equal(Now.AddDays(1), second.InvitedAt);
        Assert.Equal(2, album.SeatsTaken);
    }

    [Fact]
    public void Only_the_owner_invites() =>
        Assert.Equal(AlbumErrors.NoAccess, NewAlbum().Invite(Guid.CreateVersion7(), Guid.CreateVersion7(), "a@example.invalid", 10, Now).Error);

    [Fact]
    public void Accepting_turns_the_invitation_into_a_participant_on_the_same_seat()
    {
        var album = NewAlbum();
        var person = Guid.CreateVersion7();
        var invitation = album.Invite(Guid.CreateVersion7(), Owner, "a@example.invalid", 2, Now).Value;

        Assert.True(album.Accept(invitation.Id, person, Now).IsSuccess);

        Assert.True(album.HasMember(person));
        Assert.Equal(2, album.SeatsTaken);
        Assert.Contains(album.DomainEvents, item => item is InvitationAccepted accepted && accepted.AccountId == person);
    }

    [Fact]
    public void An_answer_comes_once()
    {
        var album = NewAlbum();
        var invitation = album.Invite(Guid.CreateVersion7(), Owner, "a@example.invalid", 2, Now).Value;
        album.Decline(invitation.Id, Now);

        Assert.Equal(AlbumErrors.InvitationGone, album.Accept(invitation.Id, Guid.CreateVersion7(), Now).Error);
        Assert.Equal(AlbumErrors.InvitationGone, album.Decline(invitation.Id, Now).Error);
    }

    [Fact]
    public void Declining_withdrawing_leaving_and_removing_free_the_seat_at_once()
    {
        var album = NewAlbum();
        var declined = album.Invite(Guid.CreateVersion7(), Owner, "a@example.invalid", 10, Now).Value;
        var withdrawn = album.Invite(Guid.CreateVersion7(), Owner, "b@example.invalid", 10, Now).Value;
        var leaving = album.Invite(Guid.CreateVersion7(), Owner, "c@example.invalid", 10, Now).Value;
        var removed = album.Invite(Guid.CreateVersion7(), Owner, "d@example.invalid", 10, Now).Value;
        var (first, second) = (Guid.CreateVersion7(), Guid.CreateVersion7());
        album.Accept(leaving.Id, first, Now);
        album.Accept(removed.Id, second, Now);
        Assert.Equal(5, album.SeatsTaken);

        Assert.True(album.Decline(declined.Id, Now).IsSuccess);
        Assert.Equal(4, album.SeatsTaken);
        Assert.True(album.Withdraw(withdrawn.Id, Owner, Now).IsSuccess);
        Assert.Equal(3, album.SeatsTaken);
        Assert.True(album.Leave(first, Now).IsSuccess);
        Assert.Equal(2, album.SeatsTaken);
        Assert.True(album.RemoveParticipant(Owner, second, Now).IsSuccess);
        Assert.Equal(1, album.SeatsTaken);
    }

    [Fact]
    public void The_owner_can_neither_leave_nor_be_removed()
    {
        var album = NewAlbum();

        Assert.Equal(AlbumErrors.OwnerCannotLeave, album.Leave(Owner, Now).Error);
        Assert.Equal(AlbumErrors.OwnerCannotLeave, album.RemoveParticipant(Owner, Owner, Now).Error);
    }

    [Fact]
    public void Only_the_owner_removes_people()
    {
        var album = NewAlbum();
        var person = Guid.CreateVersion7();
        album.AddParticipant(person);

        Assert.Equal(AlbumErrors.NoAccess, album.RemoveParticipant(person, person, Now).Error);
    }

    [Fact]
    public void A_successor_is_someone_in_the_album_and_stops_being_one_on_leaving()
    {
        var album = NewAlbum();
        var person = Guid.CreateVersion7();

        Assert.Equal(AlbumErrors.SuccessorNotParticipant, album.NameSuccessor(Owner, person, Now).Error);
        album.AddParticipant(person);
        Assert.Equal(AlbumErrors.NoAccess, album.NameSuccessor(person, person, Now).Error);
        Assert.True(album.NameSuccessor(Owner, person, Now).IsSuccess);
        Assert.Equal(person, album.SuccessorAccountId);

        album.Leave(person, Now);

        Assert.Null(album.SuccessorAccountId);
    }

    [Fact]
    public void Seven_days_mark_an_invitation_once_and_only_an_unanswered_one()
    {
        var album = NewAlbum();
        var invitation = album.Invite(Guid.CreateVersion7(), Owner, "a@example.invalid", 10, Now).Value;
        album.ClearDomainEvents();

        album.MarkWeekPassed(invitation.Id, Now.AddDays(7));
        album.MarkWeekPassed(invitation.Id, Now.AddDays(8));
        Assert.IsType<InvitationWeekPassed>(Assert.Single(album.DomainEvents));

        var answered = album.Invite(Guid.CreateVersion7(), Owner, "b@example.invalid", 10, Now).Value;
        album.Decline(answered.Id, Now);
        album.ClearDomainEvents();
        album.MarkWeekPassed(answered.Id, Now.AddDays(7));
        Assert.Empty(album.DomainEvents);
    }

    [Theory]
    [InlineData("  Ania@Example.Invalid ", "ania@example.invalid")]
    [InlineData("nie-adres", null)]
    [InlineData("a@@b", null)]
    [InlineData("a b@c", null)]
    [InlineData("", null)]
    public void Addresses_have_one_spelling(string input, string? expected) =>
        Assert.Equal(expected, InvitationEmail.Normalize(input));

    private static SharedAlbum NewAlbum() => SharedAlbum.Start(new AlbumId(Guid.CreateVersion7()), Owner, "Rodzina", Now).Value;

    private static (SharedAlbum Album, Guid First, Guid Second) Ownerless()
    {
        var album = NewAlbum();
        var (first, second) = (Guid.CreateVersion7(), Guid.CreateVersion7());
        album.AddParticipant(first);
        album.AddParticipant(second);
        album.ReleaseOwner(Now);
        return (album, first, second);
    }

    [Fact]
    public void A_late_payment_counts_for_whoever_held_it_for_a_day()
    {
        var (album, first, _) = Ownerless();
        album.ReserveTakeover(first, Now);
        album.ExpireReservation(Now + SharedAlbum.ReservationTime);

        Assert.True(album.TakeOver(Now + SharedAlbum.ReservationTime + TimeSpan.FromHours(3)).IsSuccess);
        Assert.True(album.IsOwner(first));
    }

    [Fact]
    public void After_the_day_a_payment_gives_nothing()
    {
        var (album, first, _) = Ownerless();
        album.ReserveTakeover(first, Now);
        var late = Now + SharedAlbum.ReservationTime + SharedAlbum.LatePaymentGrace;

        Assert.Equal(AlbumErrors.TakeoverExpired, album.TakeOver(late).Error);
        album.ExpireReservation(late);
        Assert.Null(album.ReservedByAccountId);
        Assert.True(album.IsOwnerless);
    }

    [Fact]
    public void Someone_who_reserved_since_takes_the_place_of_the_late_one()
    {
        var (album, first, second) = Ownerless();
        album.ReserveTakeover(first, Now);
        var later = Now + SharedAlbum.ReservationTime;

        Assert.True(album.ReserveTakeover(second, later).IsSuccess);
        Assert.True(album.TakeOver(later.AddMinutes(1)).IsSuccess);
        Assert.True(album.IsOwner(second));
    }
}
