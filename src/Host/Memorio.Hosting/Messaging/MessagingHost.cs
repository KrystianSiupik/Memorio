namespace Memorio.Hosting.Messaging;

/// <summary>Which side of the outbox a host is (ADR 0006, D-036).</summary>
public enum MessagingHost
{
    /// <summary>The API: stores integration events in the outbox and sends them to the queue; handles none.</summary>
    Api,

    /// <summary>The worker: receives from the queue through the inbox and runs the modules' handlers.</summary>
    Worker,
}
