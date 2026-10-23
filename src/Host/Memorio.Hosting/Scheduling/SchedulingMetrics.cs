using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Memorio.Hosting.Scheduling;

/// <summary>
/// Meter <see cref="MeterName"/>: <c>memorio.scheduling.runs</c>, firings per <c>scheduled_job</c> (not "job",
/// which Prometheus uses for the service) and <c>outcome</c>
/// (<see cref="Succeeded"/>, <see cref="Failed"/> and retried, <see cref="GivenUp"/> after the last retry,
/// <see cref="Unknown"/> job name). Alert on any given-up or unknown firing (monitoring/prometheus/rules).
/// </summary>
internal sealed class SchedulingMetrics : IDisposable
{
    public const string MeterName = "Memorio.Scheduling";
    public const string Succeeded = "succeeded";
    public const string Failed = "failed";
    public const string GivenUp = "given_up";
    public const string Unknown = "unknown";

    private readonly Meter _meter = new(MeterName);
    private readonly Counter<long> _runs;

    public SchedulingMetrics()
    {
        _runs = _meter.CreateCounter<long>("memorio.scheduling.runs", description: "Firings of scheduled jobs by outcome.");
    }

    public void Record(string job, string outcome) =>
        _runs.Add(1, new TagList { { "scheduled_job", job }, { "outcome", outcome } });

    public void Dispose() => _meter.Dispose();
}
