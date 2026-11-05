using Memorio.Modules.Notifications.Contracts;
using Memorio.Modules.Plans.Domain.Cycle;
using Memorio.Modules.Plans.Domain.Entitlements;
using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Plans.Application.Policies;

/// <summary>E98 is the proof the trim needs (S2:B1), and it ends a hold; a report about another round counts for nothing (D-013).</summary>
internal sealed class RecordWarningDelivered(IPlanCycles cycles, TimeProvider clock) : IIntegrationEventPolicy<TrimWarningDeliveredEvent>
{
    public async Task HandleAsync(TrimWarningDeliveredEvent integrationEvent, CancellationToken cancellationToken)
    {
        var resource = new EntitledResource(integrationEvent.IsAlbum ? EntitledResourceKind.Album : EntitledResourceKind.Account, integrationEvent.ResourceId);
        var cycle = await cycles.FindAsync(resource, cancellationToken);
        cycle?.RecordWarningDelivered(integrationEvent.DaysBefore, integrationEvent.Round, clock.GetUtcNow());
        if (!await cycles.SaveAsync(cancellationToken))
        {
            throw new InvalidOperationException("Another decision about this cycle got there first; the retry reads it again.");
        }
    }
}

/// <summary>P16 and C93: whenever a warning does not arrive, the trim waits (S2:C10).</summary>
internal sealed class HoldTrimWhenUndelivered(IPlanCycles cycles, TimeProvider clock) : IIntegrationEventPolicy<TrimWarningUndeliveredEvent>
{
    public async Task HandleAsync(TrimWarningUndeliveredEvent integrationEvent, CancellationToken cancellationToken)
    {
        var resource = new EntitledResource(integrationEvent.IsAlbum ? EntitledResourceKind.Album : EntitledResourceKind.Account, integrationEvent.ResourceId);
        var cycle = await cycles.FindAsync(resource, cancellationToken);
        cycle?.HoldTrim(integrationEvent.DaysBefore, integrationEvent.Round, clock.GetUtcNow());
        if (!await cycles.SaveAsync(cancellationToken))
        {
            throw new InvalidOperationException("Another decision about this cycle got there first; the retry reads it again.");
        }
    }
}
