using System.Globalization;
using Memorio.Modules.Notifications.Application.Commands;
using Memorio.Modules.Plans.Contracts;
using Memorio.SharedKernel.Messaging;
using Memorio.SharedKernel.Scheduling;

namespace Memorio.Modules.Notifications.Application.Policies;

/// <summary>P15: whenever a warning is due, warn the owner and the participants.</summary>
internal sealed class WarnWhenDue(WarnBeforeTrimHandler warn) : IIntegrationEventPolicy<TrimWarningDueEvent>
{
    public async Task HandleAsync(TrimWarningDueEvent integrationEvent, CancellationToken cancellationToken) =>
        await warn.HandleAsync(
            new WarnBeforeTrim(
                integrationEvent.ResourceId,
                integrationEvent.IsAlbum,
                integrationEvent.DaysBefore,
                integrationEvent.Round,
                integrationEvent.DaysBefore == 1 ? "1 dzień" : $"{integrationEvent.DaysBefore} dni"),
            cancellationToken);
}

/// <summary>P26: whenever a trim is held, try again every month until the warning arrives; a paid plan ends it (HS-010).</summary>
internal sealed class RetryWhileHeld(IDeadlines deadlines)
    : IIntegrationEventPolicy<TrimHeldEvent>, IIntegrationEventPolicy<PlanActivatedEvent>
{
    public const string JobName = "Notifications.TrimWarningRetry";
    public const string IsAlbumKey = "isAlbum";
    public const string DaysKey = "days";
    public const string RoundKey = "round";

    public Task HandleAsync(TrimHeldEvent integrationEvent, CancellationToken cancellationToken) =>
        ScheduleAsync(integrationEvent.ResourceId, integrationEvent.IsAlbum, integrationEvent.DaysBefore, integrationEvent.Round, integrationEvent.OccurredAt.AddMonths(1), cancellationToken);

    public Task HandleAsync(PlanActivatedEvent integrationEvent, CancellationToken cancellationToken) =>
        deadlines.CancelAsync(JobName, integrationEvent.ResourceId.ToString(), cancellationToken);

    public Task ScheduleAsync(Guid resourceId, bool isAlbum, int days, int round, DateTimeOffset at, CancellationToken cancellationToken) =>
        deadlines.ScheduleAsync(
            JobName,
            resourceId.ToString(),
            at,
            new Dictionary<string, string>
            {
                [IsAlbumKey] = isAlbum.ToString(CultureInfo.InvariantCulture),
                [DaysKey] = days.ToString(CultureInfo.InvariantCulture),
                [RoundKey] = round.ToString(CultureInfo.InvariantCulture),
            },
            cancellationToken);
}
