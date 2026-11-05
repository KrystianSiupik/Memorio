using Memorio.Modules.Plans.Application.Commands.RecordEntitlement;
using Memorio.Modules.Plans.Application.Entitlements;
using Memorio.Modules.Plans.Application.Ports;
using Memorio.Modules.Plans.Domain.Cycle;
using Memorio.Modules.Plans.Domain.Entitlements;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Plans.Application.Commands.Cycle;

/// <summary>A decision about a cycle, made again when a parallel one got there first: the loser reads the new state (D-018).</summary>
internal static class CycleDecision
{
    private const int Attempts = 3;

    public static async Task<Result> UntilStoredAsync(IPlanCycles cycles, Func<Task<Result>> decide, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < Attempts; attempt++)
        {
            var decided = await decide();
            if (decided.IsFailure)
            {
                return decided;
            }

            if (await cycles.SaveAsync(cancellationToken))
            {
                return Result.Success();
            }
        }

        return PlanCycleErrors.AlreadyDecided;
    }
}

/// <summary>
/// One step of the cycle at a given time; P13, P14, P17 and C90, C91, C95 meet here (D-280). The ledger comes first,
/// so a payment beats a trim that was due at the same moment (D-006). A second run with nothing new changes nothing.
/// </summary>
internal sealed class AdvanceCycleHandler(IPlanCycles cycles, PlanEntitlements entitlements)
{
    public async Task<Result> HandleAsync(EntitledResource resource, DateTimeOffset at, CancellationToken cancellationToken, bool synchronized = false) =>
        await CycleDecision.UntilStoredAsync(
            cycles,
            async () =>
            {
                var cycle = await cycles.FindAsync(resource, cancellationToken);
                var until = await entitlements.EntitledUntilAsync(resource, cancellationToken);
                if (cycle is null)
                {
                    if (until is null)
                    {
                        return Result.Success();
                    }

                    cycle = PlanCycle.Start(resource);
                    cycles.Add(cycle);
                }

                cycle.ApplyEntitlement(until, at);
                cycle.IssueDueWarnings(at);
                cycle.OrderTrimWhenDue(at);
                if (synchronized)
                {
                    cycle.RecordSynchronized(at);
                }

                return Result.Success();
            },
            cancellationToken);
}

/// <summary>C96: asks the source now, so whoever paid does not wait for its notification (S2:C3).</summary>
internal sealed class RefreshSubscriptionHandler(ISubscriptionSource source, RecordEntitlementHandler record, AdvanceCycleHandler advance, TimeProvider clock)
{
    public async Task<Result> HandleAsync(EntitledResource resource, CancellationToken cancellationToken)
    {
        foreach (var line in await source.FetchAsync(resource, cancellationToken))
        {
            var recorded = await record.HandleAsync(line, cancellationToken);
            if (recorded.IsFailure)
            {
                return recorded.Error;
            }
        }

        return await advance.HandleAsync(resource, clock.GetUtcNow(), cancellationToken, synchronized: true);
    }
}

/// <summary>C94.</summary>
internal sealed record KeepMaterial(EntitledResource Resource, IReadOnlyCollection<Guid> MemoryIds);

internal sealed class KeepMaterialHandler(IPlanCycles cycles, TimeProvider clock)
{
    public const int MaximumMemories = 100_000;

    public async Task<Result> HandleAsync(KeepMaterial command, CancellationToken cancellationToken) =>
        await CycleDecision.UntilStoredAsync(
            cycles,
            async () =>
            {
                var cycle = await cycles.FindAsync(command.Resource, cancellationToken);
                return cycle is null || command.MemoryIds.Count > MaximumMemories
                    ? PlanCycleErrors.ChoiceClosed
                    : cycle.KeepMaterial(command.MemoryIds, clock.GetUtcNow());
            },
            cancellationToken);
}

/// <summary>C165 (D-004).</summary>
internal sealed class ActivateAfterTakeoverHandler(IPlanCycles cycles, PlanEntitlements entitlements, TimeProvider clock)
{
    public async Task<Result> HandleAsync(Guid albumId, int albumSequence, CancellationToken cancellationToken)
    {
        var resource = new EntitledResource(EntitledResourceKind.Album, albumId);
        return await CycleDecision.UntilStoredAsync(
            cycles,
            async () =>
            {
                var now = clock.GetUtcNow();
                var cycle = await cycles.FindAsync(resource, cancellationToken);
                if (cycle is null)
                {
                    return PlanCycleErrors.NotPaid;
                }

                cycle.ApplyEntitlement(await entitlements.EntitledUntilAsync(resource, cancellationToken), now);
                return cycle.ActivateAfterTakeover(albumSequence, now);
            },
            cancellationToken);
    }
}

/// <summary>The C91 of P36: an album without an owner, also one that never had a paid plan and so has no cycle yet.</summary>
internal sealed class MarkAlbumOwnerlessHandler(IPlanCycles cycles, TimeProvider clock)
{
    public async Task<Result> HandleAsync(Guid albumId, int albumSequence, CancellationToken cancellationToken)
    {
        var resource = new EntitledResource(EntitledResourceKind.Album, albumId);
        return await CycleDecision.UntilStoredAsync(
            cycles,
            async () =>
            {
                var cycle = await cycles.FindAsync(resource, cancellationToken);
                if (cycle is null)
                {
                    cycle = PlanCycle.Start(resource);
                    cycles.Add(cycle);
                }

                cycle.MarkOwnerless(albumSequence, clock.GetUtcNow());
                return Result.Success();
            },
            cancellationToken);
    }
}
