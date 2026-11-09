using Memorio.Modules.AiNarration.Domain.Summaries;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Wolverine.EntityFrameworkCore;
using Wolverine.Runtime;

namespace Memorio.Modules.AiNarration.Infrastructure.Persistence;

internal sealed class PeriodSummaries(IDbContextOutbox<AiNarrationDbContext> outbox) : IPeriodSummaries
{
    private AiNarrationDbContext Database => outbox.DbContext;

    public Task<PeriodSummary?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        Database.PeriodSummaries.FirstOrDefaultAsync(summary => summary.Id == id, cancellationToken);

    public async Task<PeriodSummary> AddOrFindAsync(PeriodSummary summary, CancellationToken cancellationToken)
    {
        Database.PeriodSummaries.Add(summary);
        if (await SaveAsync(cancellationToken))
        {
            return summary;
        }

        // The same request twice at once.
        return await FindAsync(summary.Id, cancellationToken) ?? throw new InvalidOperationException("The summary that won the race is gone.");
    }

    public async Task<bool> SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Every save flushes (D-278). The summary's events stay inside the module; Provenance hears of it through its port.
            await outbox.SaveChangesAndFlushMessagesAsync(MultiFlushMode.AllowMultiples, cancellationToken);
        }
        catch (DbUpdateException exception) when (exception is DbUpdateConcurrencyException
            || exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            if (Database.Database.CurrentTransaction is { } transaction)
            {
                await transaction.RollbackAsync(cancellationToken);
                await transaction.DisposeAsync();
            }

            Database.ChangeTracker.Clear();
            return false;
        }

        foreach (var entry in Database.ChangeTracker.Entries<PeriodSummary>())
        {
            entry.Entity.ClearDomainEvents();
        }

        return true;
    }
}
