namespace Memorio.Hosting.Messaging;

/// <summary>Configuration section "Messaging" (ADR 0006): where the outbox, inbox and queues live.</summary>
public sealed class MessagingOptions
{
    public const string Section = "Messaging";

    /// <summary>
    /// PostgreSQL connection string of the role memorio_messaging, which owns the schemas
    /// <see cref="MemorioMessaging.EnvelopeSchema"/> and <see cref="MemorioMessaging.QueueSchema"/>. Required.
    /// </summary>
    public string? ConnectionString { get; set; }
}
