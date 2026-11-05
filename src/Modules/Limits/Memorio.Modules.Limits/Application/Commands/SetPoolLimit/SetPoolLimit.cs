using Memorio.Modules.Limits.Domain.Pools;

namespace Memorio.Modules.Limits.Application.Commands.SetPoolLimit;

/// <summary>C152. <paramref name="Sequence"/> comes from the plan; only a newer one overwrites (D-016).</summary>
internal sealed record SetPoolLimit(Guid PoolId, long LimitBytes, int AiLimit, long Sequence);

internal sealed class SetPoolLimitHandler(IPools pools, TimeProvider clock)
{
    /// <summary>Null when a newer limit is already set: a repeated or late event changes nothing.</summary>
    public async Task<PoolLimitSet?> HandleAsync(SetPoolLimit command, CancellationToken cancellationToken) =>
        await pools.SetLimitAsync(command.PoolId, command.LimitBytes, command.AiLimit, command.Sequence, cancellationToken)
            ? new PoolLimitSet(Guid.CreateVersion7(), clock.GetUtcNow(), command.PoolId, command.LimitBytes, command.Sequence)
            : null;
}
