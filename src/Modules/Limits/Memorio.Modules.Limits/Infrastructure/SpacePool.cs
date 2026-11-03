using Memorio.Modules.Limits.Application.Commands.ReleaseUnits;
using Memorio.Modules.Limits.Application.Commands.ReserveUnits;
using Memorio.Modules.Limits.Contracts;

namespace Memorio.Modules.Limits.Infrastructure;

internal sealed class SpacePool(ReserveUnitsHandler reserve, ReleaseUnitsHandler release) : ISpacePool
{
    public async Task<bool> TryReserveAsync(Guid accountId, Guid operationKey, long bytes, CancellationToken cancellationToken) =>
        (await reserve.HandleAsync(new ReserveUnits(accountId, operationKey, bytes), cancellationToken)).IsSuccess;

    public Task ReleaseAsync(Guid operationKey, CancellationToken cancellationToken) =>
        release.HandleAsync(new ReleaseUnits(operationKey), cancellationToken);
}
