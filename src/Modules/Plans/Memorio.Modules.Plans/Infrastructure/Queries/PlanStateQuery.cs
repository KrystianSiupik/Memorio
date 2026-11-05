using Dapper;
using Memorio.Modules.Plans.Application.Queries;
using Memorio.Modules.Plans.Domain.Cycle;
using Memorio.Modules.Plans.Domain.Entitlements;
using Memorio.Modules.Plans.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Plans.Infrastructure.Queries;

internal sealed class PlanStateQuery(PlansDbContext database) : IPlanStateQuery
{
    private const string Sql = """
        SELECT mode AS "Mode", entitled_until AS "EntitledUntil", read_only_since AS "ReadOnlySince", trim_at AS "TrimAt",
               held_since AS "HeldSince", ledger_checked_at AS "LedgerCheckedAt", kept_memory_ids AS "KeptMemoryIds"
        FROM plans.plan_cycles
        WHERE resource_kind = @kind AND resource_id = @id
        """;

    public async Task<PlanState> GetAsync(EntitledResource resource, CancellationToken cancellationToken)
    {
        var row = await database.Database.GetDbConnection().QuerySingleOrDefaultAsync<Row>(new CommandDefinition(
            Sql,
            new { kind = resource.Kind.ToString(), id = resource.Id },
            cancellationToken: cancellationToken));
        if (row is null)
        {
            return new PlanState(nameof(PlanMode.Free), null, null, null, null, false, null, []);
        }

        var trimAt = At(row.TrimAt);
        return new PlanState(row.Mode, At(row.EntitledUntil), At(row.ReadOnlySince), trimAt, trimAt - PlanCycle.ChoiceWindow, row.HeldSince is not null, At(row.LedgerCheckedAt), row.KeptMemoryIds);
    }

    private static DateTimeOffset? At(DateTime? value) => value is { } at ? new DateTimeOffset(DateTime.SpecifyKind(at, DateTimeKind.Utc)) : null;

    // Dapper reads timestamptz as DateTime and an array only into a property, not a constructor argument.
    private sealed class Row
    {
        public string Mode { get; set; } = "";

        public DateTime? EntitledUntil { get; set; }

        public DateTime? ReadOnlySince { get; set; }

        public DateTime? TrimAt { get; set; }

        public DateTime? HeldSince { get; set; }

        public DateTime? LedgerCheckedAt { get; set; }

        public Guid[] KeptMemoryIds { get; set; } = [];
    }
}
