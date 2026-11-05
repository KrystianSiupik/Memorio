using System.Globalization;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Notifications.Application.Ports;
using Memorio.Modules.Succession.Contracts;
using Memorio.SharedKernel.Messaging;
using Memorio.SharedKernel.Scheduling;

namespace Memorio.Modules.Notifications.Application.Policies;

/// <summary>P43: the successor hears about the offer, and two reminders wait (HS-021).</summary>
internal sealed class NotifySuccessor(IAccountDirectory directory, IDeadlines deadlines, SendOnce send) : IIntegrationEventPolicy<TakeoverOfferedEvent>
{
    public const string JobName = "Notifications.SuccessorReminder";
    public const string DaysKey = "days";

    private static readonly int[] ReminderDays = [7, 21];

    public async Task HandleAsync(TakeoverOfferedEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (await directory.EmailOfAsync(integrationEvent.SuccessorAccountId, cancellationToken) is { } email)
        {
            await send.SendAsync($"offer:{integrationEvent.CaseId}", integrationEvent.Sequence, email, NotificationKind.TakeoverOffered, integrationEvent.AlbumId, cancellationToken);
        }

        foreach (var days in ReminderDays)
        {
            await deadlines.ScheduleAsync(
                JobName,
                $"{integrationEvent.CaseId}:{days}",
                integrationEvent.OccurredAt.AddDays(days),
                new Dictionary<string, string> { [DaysKey] = days.ToString(CultureInfo.InvariantCulture) },
                cancellationToken);
        }
    }
}

/// <summary>C167: only an offer that still waits.</summary>
internal sealed class RemindSuccessor(ITakeoverOffers offers, IAccountDirectory directory, SendOnce send)
{
    public async Task HandleAsync(Guid caseId, int days, CancellationToken cancellationToken)
    {
        if (await offers.FindOpenAsync(caseId, cancellationToken) is { } offer
            && await directory.EmailOfAsync(offer.SuccessorAccountId, cancellationToken) is { } email)
        {
            await send.SendAsync($"offer:{caseId}:{days}", 1, email, NotificationKind.SuccessorReminder, offer.AlbumId, cancellationToken);
        }
    }
}

/// <summary>E122: whoever reported hears it was not confirmed.</summary>
internal sealed class NotifyOfRejectedReport(IAccountDirectory directory, SendOnce send) : IIntegrationEventPolicy<DeathReportRejectedEvent>
{
    public async Task HandleAsync(DeathReportRejectedEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (await directory.EmailOfAsync(integrationEvent.ReportedByAccountId, cancellationToken) is { } email)
        {
            await send.SendAsync($"report:{integrationEvent.CaseId}", integrationEvent.Sequence, email, NotificationKind.DeathReportRejected, integrationEvent.AlbumId, cancellationToken);
        }
    }
}

/// <summary>E126: the contact hears they may read. The owner is not named in the message: only after signing in, on this address.</summary>
internal sealed class NotifyLegacyContact(ILegacyContacts contacts, SendOnce send) : IIntegrationEventPolicy<PrivateArchiveSharedEvent>
{
    public async Task HandleAsync(PrivateArchiveSharedEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (await contacts.ContactOfAsync(integrationEvent.OwnerAccountId, cancellationToken) is { } email)
        {
            await send.SendAsync($"legacy:{integrationEvent.OwnerAccountId}", integrationEvent.Sequence, email, NotificationKind.PrivateArchiveShared, Guid.Empty, cancellationToken);
        }
    }
}
