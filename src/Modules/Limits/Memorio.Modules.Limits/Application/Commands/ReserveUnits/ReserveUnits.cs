using Memorio.Modules.Limits.Domain.Pools;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Limits.Application.Commands.ReserveUnits;

/// <summary>C150: another module asks for space, once per content.</summary>
internal sealed record ReserveUnits(Guid AccountId, Guid OperationKey, long Bytes);

internal sealed class ReserveUnitsHandler(IPools pools, TimeProvider clock)
{
    public async Task<Result<UnitsReserved>> HandleAsync(ReserveUnits command, CancellationToken cancellationToken)
    {
        if (command.Bytes <= 0)
        {
            return PoolErrors.InvalidUnits;
        }

        var now = clock.GetUtcNow();
        return await pools.ReserveAsync(command.AccountId, command.OperationKey, command.Bytes, now, cancellationToken)
            ? new UnitsReserved(Guid.CreateVersion7(), now, command.AccountId, command.OperationKey, command.Bytes)
            : PoolErrors.PoolFull;
    }
}
