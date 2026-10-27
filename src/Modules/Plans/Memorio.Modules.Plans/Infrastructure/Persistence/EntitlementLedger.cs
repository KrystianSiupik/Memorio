using Memorio.Modules.Plans.Domain.Entitlements;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Memorio.Modules.Plans.Infrastructure.Persistence;

internal sealed class EntitlementLedger(PlansDbContext database) : IEntitlementLedger
{
    public async Task<bool> AppendAsync(EntitlementEntry entry, CancellationToken cancellationToken)
    {
        database.EntitlementEntries.Add(entry);
        try
        {
            await database.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // The same notification of the source, recorded before (or at the same moment by another delivery).
            database.Entry(entry).State = EntityState.Detached;
            return false;
        }
    }

    public async Task<IReadOnlyList<EntitlementEntry>> EntriesOfAsync(EntitledResource resource, CancellationToken cancellationToken) =>
        await database.EntitlementEntries
            .AsNoTracking()
            .Where(line => line.ResourceKind == resource.Kind && line.ResourceId == resource.Id)
            .ToListAsync(cancellationToken);
}
