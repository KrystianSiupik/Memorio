using Memorio.Modules.Archive.Application.Queries.TrimMaterial;
using Memorio.Modules.Archive.Domain.Memories;

namespace Memorio.Modules.Archive.Application.Commands.TrimArchive;

/// <summary>C24 for an account, on the order of Plans (S5 A7); an album keeps to itself and its authors keep their memories (D-290).</summary>
internal sealed record TrimArchive(Guid ResourceId, long LimitBytes, IReadOnlyCollection<Guid> KeptMemoryIds, int Sequence);

/// <summary>
/// The excess goes to the trash for 30 days, not away (HS-022). One transaction with its event and the sequence applied,
/// so a retry does all of it or none, and what is in the trash already is no candidate any more.
/// </summary>
internal sealed class TrimArchiveHandler(IMemories memories, ITrimMaterialQuery material, TimeProvider clock)
{
    public async Task HandleAsync(TrimArchive command, CancellationToken cancellationToken)
    {
        var applied = await memories.FindAppliedPlanEventAsync(command.ResourceId, cancellationToken);
        if (applied?.Sequence >= command.Sequence)
        {
            return;
        }

        var items = await material.OfAccountAsync(command.ResourceId, cancellationToken);
        var toTrim = TrimSelection.ToTrim(items.Select(item => item.ToCandidate()), command.KeptMemoryIds, command.LimitBytes);

        var now = clock.GetUtcNow();
        foreach (var id in toTrim)
        {
            var memory = await memories.FindAsync(new MemoryId(id), cancellationToken);
            memory?.MoveToTrashByTrimming(now, command.ResourceId);
        }

        memories.RecordApplied(applied, command.ResourceId, command.Sequence);
        await memories.AnnounceAsync(new ArchiveTrimmed(Guid.CreateVersion7(), now, command.ResourceId, false, toTrim.Count, command.Sequence, [.. toTrim]));
        if (!await memories.SaveAsync(cancellationToken))
        {
            throw new InvalidOperationException("Another decision about these memories got there first; the retry reads them again.");
        }
    }
}

/// <summary>C168: the plan is paid again within the 30 days, and what was trimmed comes back (HS-022).</summary>
internal sealed record RestoreTrimmed(Guid ResourceId, int Sequence);

internal sealed class RestoreTrimmedHandler(IMemories memories, TimeProvider clock)
{
    /// <summary>A window that ended stays ended: <see cref="Memory.Restore"/> refuses it, and only that memory.</summary>
    public async Task HandleAsync(RestoreTrimmed command, CancellationToken cancellationToken)
    {
        var applied = await memories.FindAppliedPlanEventAsync(command.ResourceId, cancellationToken);
        if (applied?.Sequence >= command.Sequence)
        {
            return;
        }

        var now = clock.GetUtcNow();
        foreach (var memory in await memories.FindTrimmedForAsync(command.ResourceId, cancellationToken))
        {
            memory.Restore(now, planPaid: true);
        }

        memories.RecordApplied(applied, command.ResourceId, command.Sequence);
        if (!await memories.SaveAsync(cancellationToken))
        {
            throw new InvalidOperationException("Another decision about these memories got there first; the retry reads them again.");
        }
    }
}
