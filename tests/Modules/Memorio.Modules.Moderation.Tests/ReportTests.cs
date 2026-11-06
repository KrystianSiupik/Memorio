using Memorio.Modules.Moderation.Domain.Reports;

namespace Memorio.Modules.Moderation.Tests;

/// <summary>AG4 on its own: categories, one decision, the appeal (S2:C2).</summary>
public sealed class ReportTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 6, 12, 0, 0, TimeSpan.Zero);

    private static Report Reported(string reason = nameof(ReportReason.PersonalData), Guid? author = null) =>
        Report.Submit(new ReportId(Guid.CreateVersion7()), Guid.CreateVersion7(), ReportTarget.Memory, Guid.CreateVersion7(), author ?? Guid.CreateVersion7(), reason, "To moje dane.", Now).Value;

    [Theory]
    [InlineData(nameof(ReportReason.ChildAbuse), true)]
    [InlineData(nameof(ReportReason.Terrorism), true)]
    [InlineData(nameof(ReportReason.OtherIllegal), true)]
    [InlineData(nameof(ReportReason.SomeonesImage), false)]
    [InlineData(nameof(ReportReason.Other), false)]
    public void Illegal_is_hidden_at_once_and_due_in_a_day_disputed_stays_seen_for_a_week(string reason, bool illegal)
    {
        var report = Reported(reason);

        Assert.Equal(illegal, report.Hidden);
        Assert.Equal(illegal ? ReportCategory.Illegal : ReportCategory.Disputed, report.Category);
        Assert.Equal(Now + (illegal ? TimeSpan.FromHours(24) : TimeSpan.FromDays(7)), report.DueBy);
        Assert.IsType<ContentReported>(report.DomainEvents.First());
        Assert.Equal(illegal, report.DomainEvents.OfType<ContentHidden>().Any());
    }

    [Theory]
    [InlineData("Nonsense", "Opis")]
    [InlineData("7", "Opis")]
    [InlineData(null, "Opis")]
    [InlineData(nameof(ReportReason.Other), "  ")]
    [InlineData(nameof(ReportReason.Other), null)]
    public void What_is_not_a_report_is_refused(string? reason, string? explanation)
    {
        var refused = Report.Submit(new ReportId(Guid.CreateVersion7()), Guid.CreateVersion7(), ReportTarget.Album, Guid.CreateVersion7(), Guid.CreateVersion7(), reason, explanation, Now);

        Assert.Equal(ReportErrors.InvalidReport, refused.Error);
    }

    [Fact]
    public void Ones_own_content_and_a_public_report_without_an_address_or_a_place_are_refused()
    {
        var me = Guid.CreateVersion7();
        Assert.Equal(ReportErrors.OwnContent, Report.Submit(new ReportId(Guid.CreateVersion7()), me, ReportTarget.Memory, Guid.CreateVersion7(), me, "Other", "x", Now).Error);
        Assert.Equal(ReportErrors.InvalidReport, Report.SubmitPublic(new ReportId(Guid.CreateVersion7()), "not an address", "album Rodzina", "Other", "x", Now).Error);
        Assert.Equal(ReportErrors.InvalidReport, Report.SubmitPublic(new ReportId(Guid.CreateVersion7()), "a@example.invalid", " ", "Other", "x", Now).Error);
        Assert.True(Report.SubmitPublic(new ReportId(Guid.CreateVersion7()), "A@Example.invalid", "album Rodzina", "Copyright", "Moje zdjęcie.", Now).IsSuccess);
    }

    [Fact]
    public void One_decision_and_removed_hides_while_kept_shows_again()
    {
        var disputed = Reported();
        disputed.ClearDomainEvents();
        Assert.True(disputed.Decide(Verdict.Removed, "Dane osobowe bez zgody.", "obsluga", Now).IsSuccess);
        Assert.Single(disputed.DomainEvents.OfType<ContentHidden>());
        Assert.Equal(Now + Report.AppealWindow, disputed.AppealUntil);
        Assert.Equal(ReportErrors.AlreadyDecided, disputed.Decide(Verdict.Kept, "Jednak nie.", "obsluga", Now).Error);

        var illegal = Reported(nameof(ReportReason.Threat));
        illegal.ClearDomainEvents();
        Assert.True(illegal.Decide(Verdict.Kept, "To nie jest groźba.", "obsluga", Now).IsSuccess);
        Assert.Single(illegal.DomainEvents.OfType<ContentRestored>());
        Assert.False(illegal.Hidden);
        Assert.Equal(Now + Report.AppealWindow, illegal.AppealUntil);
    }

    [Fact]
    public void A_decision_needs_its_reasons()
    {
        Assert.Equal(ReportErrors.InvalidDecision, Reported().Decide(Verdict.Removed, " ", "obsluga", Now).Error);
        Assert.Equal(ReportErrors.InvalidDecision, Reported().Decide(Verdict.Removed, "Powód.", "", Now).Error);
    }

    [Fact]
    public void Only_the_author_appeals_a_removal_within_the_window_once_and_a_reversal_shows_it_again()
    {
        var author = Guid.CreateVersion7();
        var report = Reported(author: author);
        report.Decide(Verdict.Removed, "Dane osobowe.", "obsluga", Now);

        Assert.Equal(ReportErrors.NoAccess, report.Appeal(Guid.CreateVersion7(), "Nie zgadzam się.", Now).Error);
        Assert.Equal(ReportErrors.AlreadyDecided, report.Appeal(author, "Za późno.", Now + Report.AppealWindow).Error);
        Assert.Equal(ReportErrors.InvalidAppeal, report.Appeal(author, "", Now).Error);
        Assert.True(report.Appeal(author, "To moje własne zdjęcie.", Now).IsSuccess);
        Assert.Equal(ReportErrors.AlreadyDecided, report.Appeal(author, "Jeszcze raz.", Now).Error);

        report.ClearDomainEvents();
        Assert.True(report.DecideAppeal(reversed: true, "Autor ma zgodę.", "obsluga2", Now).IsSuccess);
        Assert.Single(report.DomainEvents.OfType<ContentRestored>());
        Assert.Equal(ReportState.Closed, report.State);
        Assert.Equal(ReportErrors.AlreadyDecided, report.DecideAppeal(reversed: false, "x", "obsluga", Now).Error);
    }

    [Fact]
    public void The_reporter_appeals_keeping_and_a_reversal_hides_it()
    {
        var reporter = Guid.CreateVersion7();
        var report = Report.Submit(new ReportId(Guid.CreateVersion7()), reporter, ReportTarget.Memory, Guid.CreateVersion7(), Guid.CreateVersion7(), "PersonalData", "Moje dane.", Now).Value;
        report.Decide(Verdict.Kept, "Brak danych.", "obsluga", Now);

        Assert.Equal(Now + Report.AppealWindow, report.AppealUntil);
        Assert.True(report.Appeal(reporter, "Na zdjęciu jest mój adres.", Now).IsSuccess);
        Assert.Equal(true, report.AppealByReporter);
        report.ClearDomainEvents();
        Assert.True(report.DecideAppeal(reversed: true, "Widać adres.", "obsluga2", Now).IsSuccess);

        Assert.True(report.Hidden);
        Assert.Single(report.DomainEvents.OfType<ContentHidden>());
        Assert.True(Assert.Single(report.DomainEvents.OfType<AppealDecided>()).ByReporter);
    }

    [Fact]
    public void An_appeal_is_decided_by_somebody_else_and_unfounded_is_only_for_keeping()
    {
        var author = Guid.CreateVersion7();
        var report = Reported(author: author);
        Assert.Equal(ReportErrors.InvalidDecision, report.Decide(Verdict.Removed, "x", "obsluga", Now, unfounded: true).Error);
        report.Decide(Verdict.Removed, "Dane osobowe.", "obsluga", Now);
        report.Appeal(author, "Nie zgadzam się.", Now);

        Assert.Equal(ReportErrors.SameDecider, report.DecideAppeal(reversed: true, "Jednak tak.", " Obsluga ", Now).Error);
        Assert.True(report.DecideAppeal(reversed: false, "Dane są widoczne.", "obsluga2", Now).IsSuccess);
    }

    [Fact]
    public void Kept_content_cannot_be_appealed()
    {
        var author = Guid.CreateVersion7();
        var report = Reported(author: author);
        report.Decide(Verdict.Kept, "Brak naruszenia.", "obsluga", Now);

        Assert.Equal(ReportErrors.AlreadyDecided, report.Appeal(author, "?", Now).Error);
    }
}
