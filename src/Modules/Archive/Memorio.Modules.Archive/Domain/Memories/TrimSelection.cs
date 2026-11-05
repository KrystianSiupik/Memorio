namespace Memorio.Modules.Archive.Domain.Memories;

/// <summary>A memory that may be trimmed, as much as the choice needs to know about it.</summary>
internal sealed record TrimCandidate(Guid MemoryId, DateOnly DateFrom, long SizeBytes);

/// <summary>What stays when an archive comes down to its limit (S2:B1, HS-022); pure, so the rule is testable alone.</summary>
internal static class TrimSelection
{
    /// <summary>
    /// The owner's choice first, then the oldest: in a family archive the oldest cannot be made again (S2:B1). What does
    /// not fit is passed over and smaller ones after it still stay, so the limit is used as far as possible.
    /// </summary>
    public static IReadOnlyList<Guid> ToTrim(IEnumerable<TrimCandidate> candidates, IReadOnlyCollection<Guid> kept, long limitBytes)
    {
        var chosen = kept.ToHashSet();
        var left = limitBytes;
        var trimmed = new List<Guid>();
        foreach (var candidate in candidates
            .OrderByDescending(candidate => chosen.Contains(candidate.MemoryId))
            .ThenBy(candidate => candidate.DateFrom)
            .ThenBy(candidate => candidate.MemoryId))
        {
            if (candidate.SizeBytes <= left)
            {
                left -= candidate.SizeBytes;
            }
            else
            {
                trimmed.Add(candidate.MemoryId);
            }
        }

        return trimmed;
    }
}

/// <summary>The last plan event about an account or album this module applied (D-013): an earlier one arriving late is skipped.</summary>
internal sealed class AppliedPlanEvent
{
    public AppliedPlanEvent(Guid resourceId, int sequence)
    {
        ResourceId = resourceId;
        Sequence = sequence;
    }

    public Guid ResourceId { get; private set; }

    public int Sequence { get; private set; }

    public void MoveTo(int sequence) => Sequence = Math.Max(Sequence, sequence);
}
