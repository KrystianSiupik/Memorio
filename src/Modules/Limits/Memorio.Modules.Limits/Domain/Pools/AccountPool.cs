namespace Memorio.Modules.Limits.Domain.Pools;

/// <summary>
/// AG2: the space and the AI allowance of an account. Changed only by conditional updates (D-016): the
/// row is hot, and one atomic statement needs no retries. Used above the limit happens only after the
/// limit was lowered; then nothing new fits.
/// </summary>
internal sealed class AccountPool
{
    /// <summary>The free plan until a plan says otherwise (D-078): 2 GB, no AI.</summary>
    public const long DefaultLimitBytes = 2L * 1024 * 1024 * 1024;

    public Guid AccountId { get; private set; }

    public long UsedBytes { get; private set; }

    public long LimitBytes { get; private set; } = DefaultLimitBytes;

    /// <summary>Only a newer limit overwrites (D-016).</summary>
    public long LimitSequence { get; private set; }
}
