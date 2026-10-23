using System.Collections.Concurrent;
using Memorio.SharedKernel.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Messaging.Tests;

public sealed record ProbeRecorded(Guid EventId, DateTimeOffset OccurredAt) : IIntegrationEvent;

public sealed record ModulesNotified(Guid EventId, DateTimeOffset OccurredAt) : IIntegrationEvent;

/// <summary>What the worker's handlers did, per event id.</summary>
internal sealed class HandledEvents
{
    private readonly ConcurrentDictionary<(string Handler, Guid EventId), int> _counts = new();

    public void Record(string handler, Guid eventId) => _counts.AddOrUpdate((handler, eventId), 1, (_, count) => count + 1);

    public int Count(string handler, Guid eventId) => _counts.GetValueOrDefault((handler, eventId));
}

// Handlers shaped like a module's (D-135): a public static adapter resolving internal services.

public static class ProbeRecordedHandler
{
    public static void Handle(ProbeRecorded probe, IServiceProvider services) =>
        services.GetRequiredService<HandledEvents>().Record(nameof(ProbeRecordedHandler), probe.EventId);
}

public static class ModulesNotifiedHandler
{
    public static void Handle(ModulesNotified notified, IServiceProvider services) =>
        services.GetRequiredService<HandledEvents>().Record(nameof(ModulesNotifiedHandler), notified.EventId);
}

public static class FailingModulesNotifiedHandler
{
    public static void Handle(ModulesNotified notified, IServiceProvider services)
    {
        services.GetRequiredService<HandledEvents>().Record(nameof(FailingModulesNotifiedHandler), notified.EventId);
        throw new InvalidOperationException("This module is broken.");
    }
}
