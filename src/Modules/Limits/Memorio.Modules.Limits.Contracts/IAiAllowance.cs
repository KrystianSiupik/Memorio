namespace Memorio.Modules.Limits.Contracts;

/// <summary>Write port for AiNarration: the monthly AI allowance of an account. What was used never comes back (S2:B1).</summary>
public interface IAiAllowance
{
    /// <summary>False when the allowance of this month is used up. The same key again changes nothing and answers true.</summary>
    Task<bool> TryConsumeAsync(Guid accountId, Guid operationKey, CancellationToken cancellationToken);
}
