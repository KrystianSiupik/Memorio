using Memorio.Modules.Limits.Application.Commands.ReleaseUnits;
using Memorio.Modules.Limits.Application.Commands.ReserveUnits;
using Memorio.Modules.Limits.Contracts;
using Memorio.Modules.Limits.Domain.Pools;

namespace Memorio.Modules.Limits.Infrastructure;

/// <summary>An album's pool is the same AG2 under the album's id: ids are UUIDs, so it never meets an account's.</summary>
internal sealed class AlbumPool(ReserveUnitsHandler reserve, ReleaseUnitsHandler release, IPools pools) : IAlbumPool
{
    public async Task<bool> TryReserveAsync(Guid albumId, Guid operationKey, long bytes, CancellationToken cancellationToken) =>
        (await reserve.HandleAsync(new ReserveUnits(albumId, operationKey, bytes), cancellationToken)).IsSuccess;

    public Task ReleaseAsync(Guid operationKey, CancellationToken cancellationToken) =>
        release.HandleAsync(new ReleaseUnits(operationKey), cancellationToken);

    public async Task<AlbumPoolState> GetStateAsync(Guid albumId, CancellationToken cancellationToken)
    {
        var (used, limit) = await pools.GetUsageAsync(albumId, cancellationToken);
        return new AlbumPoolState(used, limit);
    }
}
