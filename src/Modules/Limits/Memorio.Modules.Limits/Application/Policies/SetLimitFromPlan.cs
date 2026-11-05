using Memorio.Modules.Limits.Application.Commands.SetPoolLimit;
using Memorio.Modules.Plans.Contracts;
using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Limits.Application.Policies;

/// <summary>
/// P27: the plan says what a pool may hold, and this pool follows it. The ordered trim brings a pool back to the free
/// limit too (D-282). A lowered limit never takes anything away: nothing new fits, and that is all (S2:B1).
/// </summary>
internal sealed class SetLimitFromPlan(SetPoolLimitHandler limits)
    : IIntegrationEventPolicy<PlanActivatedEvent>, IIntegrationEventPolicy<PlanReadOnlyEvent>, IIntegrationEventPolicy<TrimOrderedEvent>
{
    public Task HandleAsync(PlanActivatedEvent integrationEvent, CancellationToken cancellationToken) =>
        limits.HandleAsync(new SetPoolLimit(integrationEvent.ResourceId, integrationEvent.LimitBytes, integrationEvent.AiLimit, integrationEvent.Sequence), cancellationToken);

    public Task HandleAsync(PlanReadOnlyEvent integrationEvent, CancellationToken cancellationToken) =>
        limits.HandleAsync(new SetPoolLimit(integrationEvent.ResourceId, integrationEvent.LimitBytes, integrationEvent.AiLimit, integrationEvent.Sequence), cancellationToken);

    public Task HandleAsync(TrimOrderedEvent integrationEvent, CancellationToken cancellationToken) =>
        limits.HandleAsync(new SetPoolLimit(integrationEvent.ResourceId, integrationEvent.LimitBytes, integrationEvent.AiLimit, integrationEvent.Sequence), cancellationToken);
}
