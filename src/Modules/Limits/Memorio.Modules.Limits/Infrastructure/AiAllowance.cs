using Memorio.Modules.Limits.Contracts;
using Memorio.Modules.Limits.Domain.Pools;

namespace Memorio.Modules.Limits.Infrastructure;

internal sealed class AiAllowance(IPools pools, TimeProvider clock) : IAiAllowance
{
    public Task<bool> TryConsumeAsync(Guid accountId, Guid operationKey, CancellationToken cancellationToken) =>
        pools.ConsumeAiAsync(accountId, operationKey, clock.GetUtcNow(), cancellationToken);
}
