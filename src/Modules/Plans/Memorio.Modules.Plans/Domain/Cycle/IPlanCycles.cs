using Memorio.Modules.Plans.Domain.Entitlements;

namespace Memorio.Modules.Plans.Domain.Cycle;

internal interface IPlanCycles
{
    Task<PlanCycle?> FindAsync(EntitledResource resource, CancellationToken cancellationToken);

    void Add(PlanCycle cycle);

    /// <summary>
    /// What needs a step at <paramref name="at"/>: the ledger has news, the paid period ended, or the warnings or the
    /// trim may be due. A resource with nothing to do is not listed, however often this is asked.
    /// </summary>
    Task<IReadOnlyList<EntitledResource>> FindDueAsync(DateTimeOffset at, CancellationToken cancellationToken);

    /// <summary>
    /// Stores the changes with the events of the changed cycles, in one transaction. False when another decision
    /// about a cycle got there first: nothing was stored.
    /// </summary>
    Task<bool> SaveAsync(CancellationToken cancellationToken);
}
