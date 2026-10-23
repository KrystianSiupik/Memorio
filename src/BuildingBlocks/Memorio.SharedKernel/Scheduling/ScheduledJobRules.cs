using System.Text.RegularExpressions;

namespace Memorio.SharedKernel.Scheduling;

/// <summary>Rules every scheduled job follows, whichever module owns it (ADR 0012, SD12).</summary>
public static partial class ScheduledJobRules
{
    /// <summary>
    /// Waits before the retries of a failed firing; after the last one the firing is given up (logged
    /// and counted) and a recurring job waits for its next occurrence.
    /// </summary>
    public static IReadOnlyList<TimeSpan> RetryDelays { get; } =
        [TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(10), TimeSpan.FromHours(1)];

    /// <summary>
    /// A job's name is "Module.Job" (e.g. "Succession.CloseWindow"): it is stored with every scheduled
    /// firing, so it never changes once a job is released.
    /// </summary>
    public static bool IsValidName(string? name) => name is not null && JobName().IsMatch(name);

    [GeneratedRegex("^[A-Z][A-Za-z0-9]*\\.[A-Z][A-Za-z0-9]*$")]
    private static partial Regex JobName();
}
