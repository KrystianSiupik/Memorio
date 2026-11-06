using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Moderation.Contracts;
using Memorio.Modules.Notifications.Application.Ports;
using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Notifications.Application.Policies;

/// <summary>An order hid the author's content: they hear it, unless the authority asked that they do not (L11).</summary>
internal sealed class NotifyOfOrder(IAccountDirectory directory, SendOnce send) : IIntegrationEventPolicy<RemovalOrderExecutedEvent>
{
    public async Task HandleAsync(RemovalOrderExecutedEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (!integrationEvent.Confidential && integrationEvent.AuthorAccountId is { } author && await directory.EmailOfAsync(author, cancellationToken) is { } email)
        {
            await send.SendAsync($"order:{integrationEvent.OrderId}", integrationEvent.Sequence, email, NotificationKind.ContentHiddenByOrder, Guid.Empty, cancellationToken);
        }
    }
}

/// <summary>
/// Both sides of a report hear what happened, with the reasons (S2:C2, DSA art. 16 and 17). The author never learns who
/// reported; the reporter never learns who the author is. No content, only the reason and the reasons of the decision.
/// </summary>
internal sealed class NotifyOfModeration(IReportNotices notices, IAccountDirectory directory, SendOnce send)
    : IIntegrationEventPolicy<ContentReportedEvent>, IIntegrationEventPolicy<ReportDecidedEvent>, IIntegrationEventPolicy<AppealDecidedEvent>
{
    /// <summary>The reason a report gave, as the author reads it.</summary>
    public static string ReasonText(string reason) => reason switch
    {
        "ChildAbuse" => "wykorzystywanie dzieci",
        "Terrorism" => "treści terrorystyczne",
        "Threat" => "groźby",
        "Incitement" => "nawoływanie do przemocy lub nienawiści",
        "OtherIllegal" => "inna treść niezgodna z prawem",
        "SomeonesImage" => "wizerunek bez zgody",
        "PersonalData" => "dane osobowe",
        "Defamation" => "zniesławienie",
        "Copyright" => "naruszenie praw autorskich",
        _ => "inny powód",
    };

    public async Task HandleAsync(ContentReportedEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (await notices.NoticeOfAsync(integrationEvent.ReportId, cancellationToken) is not { } notice)
        {
            return;
        }

        await TellReporterAsync(integrationEvent.ReportId, "received", integrationEvent.Sequence, notice, NotificationKind.ReportReceived, null, cancellationToken);
        await TellAuthorAsync(
            integrationEvent.ReportId,
            "reported",
            integrationEvent.AuthorAccountId,
            integrationEvent.Sequence,
            integrationEvent.HiddenNow ? NotificationKind.ContentHiddenAfterReport : NotificationKind.ContentReported,
            ReasonText(notice.Reason),
            cancellationToken);
    }

    public async Task HandleAsync(ReportDecidedEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (await notices.NoticeOfAsync(integrationEvent.ReportId, cancellationToken) is not { } notice)
        {
            return;
        }

        var removed = integrationEvent.Verdict == "Removed";
        await TellReporterAsync(
            integrationEvent.ReportId, "decided", integrationEvent.Sequence, notice, removed ? NotificationKind.ReportActedOn : NotificationKind.ReportNotActedOn, notice.Statement, cancellationToken);
        await TellAuthorAsync(
            integrationEvent.ReportId,
            "decided",
            integrationEvent.AuthorAccountId,
            integrationEvent.Sequence,
            removed ? NotificationKind.ContentRemoved : NotificationKind.ContentKept,
            notice.Statement,
            cancellationToken);
    }

    public async Task HandleAsync(AppealDecidedEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (await notices.NoticeOfAsync(integrationEvent.ReportId, cancellationToken) is { } notice)
        {
            await TellAuthorAsync(
                integrationEvent.ReportId,
                "appeal",
                integrationEvent.AuthorAccountId,
                integrationEvent.Sequence,
                integrationEvent.Reversed ? NotificationKind.AppealReversed : NotificationKind.AppealUpheld,
                notice.AppealStatement,
                cancellationToken);
        }
    }

    private async Task TellReporterAsync(Guid reportId, string step, int sequence, ReportNotice notice, NotificationKind kind, string? detail, CancellationToken cancellationToken)
    {
        var email = notice.ReporterAccountId is { } account ? await directory.EmailOfAsync(account, cancellationToken) : notice.ReporterEmail;
        if (email is not null)
        {
            await send.SendAsync($"report:{reportId}:{step}:reporter", sequence, email, kind, Guid.Empty, cancellationToken, detail);
        }
    }

    private async Task TellAuthorAsync(Guid reportId, string step, Guid? author, int sequence, NotificationKind kind, string? detail, CancellationToken cancellationToken)
    {
        if (author is { } account && await directory.EmailOfAsync(account, cancellationToken) is { } email)
        {
            await send.SendAsync($"report:{reportId}:{step}:author", sequence, email, kind, Guid.Empty, cancellationToken, detail);
        }
    }
}
