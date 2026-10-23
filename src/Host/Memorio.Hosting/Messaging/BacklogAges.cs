namespace Memorio.Hosting.Messaging;

/// <summary>
/// Age of the oldest pending message in one stage, from repeated samples of the pending message ids:
/// a message is as old as the first sample that saw it. Wolverine's outbox and inbox tables carry no
/// creation time (D-136), so the age is exact to one sampling interval and starts again from zero when
/// the worker restarts.
/// </summary>
internal sealed class BacklogAges
{
    private readonly Dictionary<Guid, DateTimeOffset> _firstSeen = [];

    public int Count => _firstSeen.Count;

    /// <summary>Records the ids pending at <paramref name="now"/>; ids no longer pending are forgotten.</summary>
    public void Observe(IReadOnlyCollection<Guid> pending, DateTimeOffset now)
    {
        var current = pending.ToHashSet();
        foreach (var id in _firstSeen.Keys.Where(id => !current.Contains(id)).ToList())
        {
            _firstSeen.Remove(id);
        }

        foreach (var id in current)
        {
            _firstSeen.TryAdd(id, now);
        }
    }

    public TimeSpan OldestAge(DateTimeOffset now) =>
        _firstSeen.Count == 0 ? TimeSpan.Zero : now - _firstSeen.Values.Min();
}
