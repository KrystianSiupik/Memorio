using Memorio.Modules.Limits.Contracts;
using Memorio.Modules.Limits.Domain.Pools;

namespace Memorio.Modules.Limits.Infrastructure;

internal sealed class PoolState(IPools pools, TimeProvider clock) : IPoolState
{
    public Task<PoolStanding> GetAsync(Guid poolId, CancellationToken cancellationToken) =>
        pools.GetStandingAsync(poolId, clock.GetUtcNow(), cancellationToken);
}
