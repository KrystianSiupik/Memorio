namespace Memorio.Hosting.Scheduling;

/// <summary>Configuration section "Scheduling" (ADR 0012): where the worker's scheduler keeps its jobs.</summary>
public sealed class SchedulingOptions
{
    public const string Section = "Scheduling";

    /// <summary>
    /// PostgreSQL connection string of the role memorio_scheduler, which owns the schema
    /// <see cref="MemorioScheduling.Schema"/>. Required in the worker.
    /// </summary>
    public string? ConnectionString { get; set; }
}
