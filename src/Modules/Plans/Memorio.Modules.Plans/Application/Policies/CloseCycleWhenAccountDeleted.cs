using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Plans.Domain.Cycle;
using Memorio.Modules.Plans.Domain.Entitlements;
using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Plans.Application.Policies;

/// <summary>A deleted account has no cycle to run: no warnings, no trim, and no line of the ledger brings it back. Twice changes nothing.</summary>
internal sealed class CloseCycleWhenAccountDeleted(IPlanCycles cycles) : IIntegrationEventPolicy<AccountDeleted>
{
    public async Task HandleAsync(AccountDeleted integrationEvent, CancellationToken cancellationToken)
    {
        var resource = new EntitledResource(EntitledResourceKind.Account, integrationEvent.AccountId);
        var cycle = await cycles.FindAsync(resource, cancellationToken);
        if (cycle is null)
        {
            cycle = PlanCycle.Start(resource);
            cycles.Add(cycle);
        }
        else if (cycle.Mode == PlanMode.Closed)
        {
            return;
        }

        cycle.Close();
        if (!await cycles.SaveAsync(cancellationToken))
        {
            throw new InvalidOperationException("Another decision about this cycle got there first; the retry reads it again.");
        }
    }
}
