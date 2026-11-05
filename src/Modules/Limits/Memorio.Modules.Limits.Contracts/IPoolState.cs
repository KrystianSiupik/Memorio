namespace Memorio.Modules.Limits.Contracts;

/// <summary>Read port for Plans: how the pool of an account or album stands, for whoever pays for it (R16).</summary>
public interface IPoolState
{
    Task<PoolStanding> GetAsync(Guid poolId, CancellationToken cancellationToken);
}

/// <param name="AiUsed">In the current month.</param>
public sealed record PoolStanding(long UsedBytes, long LimitBytes, int AiUsed, int AiLimit);
