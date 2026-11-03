using Memorio.Modules.Limits.Domain.Pools;

namespace Memorio.Modules.Limits.Application.Commands.ReleaseUnits;

/// <summary>C151: the content is gone, and its space comes back. The AI allowance never does (S2:B1).</summary>
internal sealed record ReleaseUnits(Guid OperationKey);

internal sealed class ReleaseUnitsHandler(IPools pools, TimeProvider clock)
{
    /// <summary>Null when there was nothing to give back: safe to call again after a retry.</summary>
    public async Task<UnitsReleased?> HandleAsync(ReleaseUnits command, CancellationToken cancellationToken)
    {
        var now = clock.GetUtcNow();
        return await pools.ReleaseAsync(command.OperationKey, now, cancellationToken)
            ? new UnitsReleased(Guid.CreateVersion7(), now, command.OperationKey)
            : null;
    }
}
