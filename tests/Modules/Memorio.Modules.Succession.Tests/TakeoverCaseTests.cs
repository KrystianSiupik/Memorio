using Memorio.Modules.Succession.Domain.Cases;

namespace Memorio.Modules.Succession.Tests;

/// <summary>The rules of AG7 without a database.</summary>
public sealed class TakeoverCaseTests
{
    private static readonly DateTimeOffset Now = new(2030, 1, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid Successor = Guid.CreateVersion7();

    private static TakeoverCase Reported() =>
        TakeoverCase.Report(new TakeoverCaseId(Guid.CreateVersion7()), Guid.CreateVersion7(), Guid.CreateVersion7(), Guid.CreateVersion7(), "certificates/x/original", Now);

    private static TakeoverCase Offered()
    {
        var found = Reported();
        found.Verify(Successor, Now);
        found.ClearDomainEvents();
        return found;
    }

    [Fact]
    public void Verifying_confirms_the_death_and_makes_the_offer_for_30_days()
    {
        var found = Reported();

        Assert.True(found.Verify(Successor, Now).IsSuccess);

        Assert.IsType<DeathConfirmed>(found.DomainEvents.First());
        var offer = Assert.IsType<TakeoverOffered>(found.DomainEvents.Last());
        Assert.Equal(Now.AddDays(30), offer.AnswerBy);
        Assert.Equal(CaseState.Offered, found.State);
    }

    [Fact]
    public void Without_a_successor_it_is_as_after_a_refusal()
    {
        var found = Reported();

        found.Verify(null, Now);

        Assert.IsType<TakeoverRefused>(found.DomainEvents.Last());
        Assert.Equal(CaseState.Refused, found.State);
    }

    [Fact]
    public void One_decision_per_stage()
    {
        var found = Offered();
        Assert.Equal(CaseErrors.AlreadyDecided, found.Verify(Successor, Now).Error);
        Assert.Equal(CaseErrors.AlreadyDecided, found.Reject(Now).Error);

        Assert.True(found.Accept(Successor, Now).IsSuccess);
        Assert.Equal(CaseErrors.AlreadyDecided, found.Accept(Successor, Now).Error);
        Assert.Equal(CaseErrors.AlreadyDecided, found.Refuse(Successor, Now).Error);
    }

    [Fact]
    public void Nobody_answers_before_the_certificate_is_checked()
    {
        Assert.Equal(CaseErrors.NotVerified, Reported().Accept(Successor, Now).Error);
        Assert.Equal(CaseErrors.NotVerified, Reported().Refuse(Successor, Now).Error);
    }

    [Fact]
    public void Only_the_successor_of_the_snapshot_answers()
    {
        var found = Offered();

        Assert.Equal(CaseErrors.NotSuccessor, found.Accept(Guid.CreateVersion7(), Now).Error);
        Assert.Equal(CaseErrors.NotSuccessor, found.Refuse(Guid.CreateVersion7(), Now).Error);
    }

    [Fact]
    public void After_30_days_there_is_no_answering()
    {
        var found = Offered();

        Assert.Equal(CaseErrors.AlreadyDecided, found.Accept(Successor, Now.AddDays(30)).Error);
    }

    [Fact]
    public void No_answer_in_time_is_a_refusal_and_not_a_moment_sooner()
    {
        var found = Offered();

        Assert.Equal(CaseErrors.StillOpen, found.ExpireIfDue(Now.AddDays(30).AddSeconds(-1)).Error);
        Assert.True(found.ExpireIfDue(Now.AddDays(30)).IsSuccess);

        Assert.IsType<TakeoverRefused>(Assert.Single(found.DomainEvents));
    }

    [Fact]
    public void Accepting_names_who_gives_and_who_takes()
    {
        var found = Offered();

        found.Accept(Successor, Now);

        var moved = Assert.IsType<AlbumTransferred>(Assert.Single(found.DomainEvents));
        Assert.Equal(found.OwnerAccountId, moved.FromAccountId);
        Assert.Equal(Successor, moved.ToAccountId);
    }

    [Fact]
    public void A_report_rejected_tells_who_made_it_and_closes()
    {
        var found = Reported();

        found.Reject(Now);

        Assert.Equal(found.ReportedByAccountId, Assert.IsType<DeathReportRejected>(Assert.Single(found.DomainEvents)).ReportedByAccountId);
        Assert.Equal(Now, found.ClosedAt);
    }

    [Fact]
    public void A_contact_is_shared_once()
    {
        var contact = LegacyContact.Name(Guid.CreateVersion7(), "ania@example.invalid");

        contact.Share(Now);
        contact.Share(Now.AddDays(1));

        Assert.Equal(Now, contact.SharedAt);
        Assert.Single(contact.DomainEvents);
    }
}
