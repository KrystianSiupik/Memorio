namespace Memorio.Modules.Archive.Domain.Memories;

/// <summary>The last event about an account this module applied (D-013): an earlier one arriving late is skipped.</summary>
internal sealed class AppliedAccountEvent
{
    public AppliedAccountEvent(Guid accountId, int sequence)
    {
        AccountId = accountId;
        Sequence = sequence;
    }

    public Guid AccountId { get; private set; }

    public int Sequence { get; private set; }

    public void MoveTo(int sequence) => Sequence = Math.Max(Sequence, sequence);
}
