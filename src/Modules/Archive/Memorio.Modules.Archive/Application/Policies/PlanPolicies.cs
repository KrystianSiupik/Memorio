using Memorio.Modules.Archive.Application.Commands.TrimArchive;
using Memorio.Modules.Plans.Contracts;
using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Archive.Application.Policies;

/// <summary>P33: whenever a trim is ordered, trim by the owner's choice and otherwise from the oldest.</summary>
internal sealed class TrimWhenOrdered(TrimArchiveHandler trim) : IIntegrationEventPolicy<TrimOrderedEvent>
{
    public Task HandleAsync(TrimOrderedEvent integrationEvent, CancellationToken cancellationToken) =>
        integrationEvent.IsAlbum
            ? Task.CompletedTask
            : trim.HandleAsync(new TrimArchive(integrationEvent.ResourceId, integrationEvent.LimitBytes, integrationEvent.KeptMemoryIds, integrationEvent.Sequence), cancellationToken);
}

/// <summary>P44: whenever a plan is active again, what was trimmed comes back while its 30 days last.</summary>
internal sealed class RestoreTrimmedWhenPlanActive(RestoreTrimmedHandler restore) : IIntegrationEventPolicy<PlanActivatedEvent>
{
    public Task HandleAsync(PlanActivatedEvent integrationEvent, CancellationToken cancellationToken) =>
        integrationEvent.IsAlbum
            ? Task.CompletedTask
            : restore.HandleAsync(new RestoreTrimmed(integrationEvent.ResourceId, integrationEvent.Sequence), cancellationToken);
}
