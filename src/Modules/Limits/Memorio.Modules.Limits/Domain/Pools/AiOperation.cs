namespace Memorio.Modules.Limits.Domain.Pools;

/// <summary>One use of the AI allowance; its key is the id of the draft, so a retry costs nothing more.</summary>
internal sealed class AiOperation
{
    public Guid OperationKey { get; private set; }

    public Guid AccountId { get; private set; }

    public DateTimeOffset ConsumedAt { get; private set; }
}
