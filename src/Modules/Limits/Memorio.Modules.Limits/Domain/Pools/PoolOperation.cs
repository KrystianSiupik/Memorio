namespace Memorio.Modules.Limits.Domain.Pools;

/// <summary>One reservation; its key is the id of the content, so one operation has one effect (D-015).</summary>
internal sealed class PoolOperation
{
    public Guid OperationKey { get; private set; }

    public Guid AccountId { get; private set; }

    public long Bytes { get; private set; }

    public DateTimeOffset ReservedAt { get; private set; }

    public DateTimeOffset? ReleasedAt { get; private set; }
}
