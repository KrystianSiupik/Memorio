using System.Collections.Concurrent;
using Memorio.SharedKernel.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Messaging.Tests;

public sealed record ProbeRecorded(Guid EventId, DateTimeOffset OccurredAt) : IIntegrationEvent;

public sealed record ModulesNotified(Guid EventId, DateTimeOffset OccurredAt) : IIntegrationEvent;

public sealed record ReplayProbe(Guid EventId, DateTimeOffset OccurredAt) : IIntegrationEvent;

/// <summary>What the worker's handlers did, per event id, and the events whose handler fails.</summary>
internal sealed class HandledEvents
{
    private readonly ConcurrentDictionary<(string Handler, Guid EventId), int> _counts = new();
    private readonly ConcurrentDictionary<Guid, bool> _broken = new();

    public void Record(string handler, Guid eventId) => _counts.AddOrUpdate((handler, eventId), 1, (_, count) => count + 1);

    public int Count(string handler, Guid eventId) => _counts.GetValueOrDefault((handler, eventId));

    /// <summary>The handler of this event fails until <see cref="Repair"/> (a bug fixed by a release).</summary>
    public void Break(Guid eventId) => _broken[eventId] = true;

    public void Repair(Guid eventId) => _broken.TryRemove(eventId, out _);

    public bool IsBroken(Guid eventId) => _broken.ContainsKey(eventId);
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

public static class ReplayProbeHandler
{
    public const string Failure = "The module's bug.";

    public static void Handle(ReplayProbe probe, IServiceProvider services)
    {
        var handled = services.GetRequiredService<HandledEvents>();
        if (handled.IsBroken(probe.EventId))
        {
            throw new InvalidOperationException(Failure);
        }

        handled.Record(nameof(ReplayProbeHandler), probe.EventId);
    }
}
