using Dapper;
using Memorio.Modules.Plans.Contracts;
using Memorio.Modules.Plans.Domain.Cycle;
using Memorio.Modules.Plans.Domain.Entitlements;
using Memorio.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Wolverine.EntityFrameworkCore;
using Wolverine.Runtime;

namespace Memorio.Modules.Plans.Infrastructure.Persistence;

internal sealed class PlanCycles(IDbContextOutbox<PlansDbContext> outbox) : IPlanCycles
{
    private const string DueSql = """
        SELECT resource_kind AS "Kind", resource_id AS "Id" FROM plans.plan_cycles
        WHERE (mode = 'Active' AND entitled_until <= @at)
           OR (mode = 'ReadOnly' AND trim_at - interval '90 days' <= @at)
        UNION
        SELECT entry.resource_kind, entry.resource_id
        FROM plans.entitlement_entries entry
        LEFT JOIN plans.plan_cycles cycle ON cycle.resource_kind = entry.resource_kind AND cycle.resource_id = entry.resource_id
        WHERE cycle.resource_id IS NULL OR (cycle.mode <> 'Closed' AND entry.recorded_at > cycle.ledger_checked_at)
        """;

    private PlansDbContext Database => outbox.DbContext;

    public Task<PlanCycle?> FindAsync(EntitledResource resource, CancellationToken cancellationToken) =>
        Database.PlanCycles.FirstOrDefaultAsync(cycle => cycle.ResourceKind == resource.Kind && cycle.ResourceId == resource.Id, cancellationToken);

    public void Add(PlanCycle cycle) => Database.PlanCycles.Add(cycle);

    public async Task<IReadOnlyList<EntitledResource>> FindDueAsync(DateTimeOffset at, CancellationToken cancellationToken)
    {
        var rows = await Database.Database.GetDbConnection().QueryAsync<DueRow>(
            new CommandDefinition(DueSql, new { at }, cancellationToken: cancellationToken));
        return [.. rows.Select(row => new EntitledResource(Enum.Parse<EntitledResourceKind>(row.Kind), row.Id))];
    }

    public async Task<bool> SaveAsync(CancellationToken cancellationToken)
    {
        foreach (var cycle in Database.ChangeTracker.Entries<PlanCycle>().Select(entry => entry.Entity).ToList())
        {
            foreach (var domainEvent in cycle.DomainEvents)
            {
                await PublishAsync(domainEvent);
            }
        }

        try
        {
            // Every save flushes (D-278): by default what a second save in a scope publishes is dropped.
            await outbox.SaveChangesAndFlushMessagesAsync(MultiFlushMode.AllowMultiples, cancellationToken);
        }
        catch (DbUpdateException exception) when (exception is DbUpdateConcurrencyException
            || exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // The refused transaction must not stay open: PostgreSQL would refuse everything after a failed statement.
            if (Database.Database.CurrentTransaction is { } transaction)
            {
                await transaction.RollbackAsync(cancellationToken);
                await transaction.DisposeAsync();
            }

            Database.ChangeTracker.Clear();
            return false;
        }

        foreach (var entry in Database.ChangeTracker.Entries<PlanCycle>())
        {
            entry.Entity.ClearDomainEvents();
        }

        return true;
    }

    private ValueTask PublishAsync(IDomainEvent domainEvent) => domainEvent switch
    {
        PlanActivated plan => outbox.PublishAsync(new PlanActivatedEvent(
            plan.EventId, plan.OccurredAt, plan.Resource.Id, plan.Resource.Kind == EntitledResourceKind.Album, plan.Terms.LimitBytes, plan.Terms.AiLimit, plan.Sequence)),

        PlanWentReadOnly plan => outbox.PublishAsync(new PlanReadOnlyEvent(
            plan.EventId, plan.OccurredAt, plan.Resource.Id, plan.Resource.Kind == EntitledResourceKind.Album, plan.Terms.LimitBytes, plan.Terms.AiLimit, plan.Sequence)),

        TrimWarningDue warning => outbox.PublishAsync(new TrimWarningDueEvent(
            warning.EventId, warning.OccurredAt, warning.Resource.Id, warning.Resource.Kind == EntitledResourceKind.Album, warning.DaysBefore, warning.TrimAt, warning.Round)),

        TrimHeld held => outbox.PublishAsync(new TrimHeldEvent(
            held.EventId, held.OccurredAt, held.Resource.Id, held.Resource.Kind == EntitledResourceKind.Album, held.DaysBefore, held.Round, held.Sequence)),

        TrimOrdered trim => outbox.PublishAsync(new TrimOrderedEvent(
            trim.EventId, trim.OccurredAt, trim.Resource.Id, trim.Resource.Kind == EntitledResourceKind.Album, trim.Terms.LimitBytes, trim.Terms.AiLimit, trim.KeptMemoryIds, trim.Sequence)),

        PaidWhileOwnerless paid => outbox.PublishAsync(new PaidWhileOwnerlessEvent(paid.EventId, paid.OccurredAt, paid.Resource.Id, paid.Sequence)),

        // The others stay inside the module.
        _ => ValueTask.CompletedTask,
    };

    private sealed class DueRow
    {
        public string Kind { get; set; } = "";

        public Guid Id { get; set; }
    }
}
