using Memorio.Modules.Moderation.Domain.Reports;
using Microsoft.EntityFrameworkCore;
using Wolverine.EntityFrameworkCore;

namespace Memorio.Modules.Moderation.Infrastructure.Persistence;

internal sealed class Reports(IDbContextOutbox<ModerationDbContext> outbox) : IReports
{
    private ModerationDbContext Database => outbox.DbContext;

    public Task<Report?> FindAsync(ReportId id, CancellationToken cancellationToken) =>
        Database.Reports.FirstOrDefaultAsync(found => found.Id == id, cancellationToken);

    public Task<Report?> FindOpenAsync(Report like, CancellationToken cancellationToken) =>
        Database.Reports.FirstOrDefaultAsync(
            found => found.State == ReportState.Open && found.ReporterKey == like.ReporterKey && found.TargetKey == like.TargetKey,
            cancellationToken);

    public async Task<bool> IsSuspendedAsync(string reporterKey, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var since = now - Report.Suspension - Report.UnfoundedWindow;
        var unfounded = await Database.Reports.AsNoTracking()
            .Where(found => found.ReporterKey == reporterKey && found.Unfounded && found.DecidedAt >= since)
            .Select(found => found.DecidedAt!.Value)
            .ToListAsync(cancellationToken);
        var sorted = unfounded.Order().ToList();
        for (var last = Report.UnfoundedToSuspend - 1; last < sorted.Count; last++)
        {
            if (sorted[last] - sorted[last - Report.UnfoundedToSuspend + 1] <= Report.UnfoundedWindow && now - sorted[last] < Report.Suspension)
            {
                return true;
            }
        }

        return false;
    }

    public void Add(Report report) => Database.Reports.Add(report);

    public async Task<IReadOnlyList<Guid>> FindPendingActionsAsync(CancellationToken cancellationToken) =>
        await Database.Actions.AsNoTracking().Where(action => action.AppliedAt == null).OrderBy(action => action.DecidedAt).Select(action => action.Id).ToListAsync(cancellationToken);

    public Task<OperatorAction?> FindActionAsync(Guid id, CancellationToken cancellationToken) =>
        Database.Actions.FirstOrDefaultAsync(action => action.Id == id, cancellationToken);

    public Task<bool> SaveAsync(CancellationToken cancellationToken) => ModerationSaving.SaveAsync(outbox, cancellationToken);
}
