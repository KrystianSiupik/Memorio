namespace Memorio.SharedKernel.Messaging;

/// <summary>
/// A module's policy reacting to another module's integration event ("whenever X happened, do Y"; the
/// P-policies of the process model), in its Application layer (internal). The worker calls it through
/// the module's public handler in Infrastructure/Messaging. Delivery is at least once: handling the same
/// event twice must leave the same state as handling it once (SD6).
/// </summary>
public interface IIntegrationEventPolicy<in TEvent>
    where TEvent : IIntegrationEvent
{
    Task HandleAsync(TEvent integrationEvent, CancellationToken cancellationToken);
}
