namespace Memorio.Modules.Archive.Contracts;

/// <summary>Read port for the Account module: how much of what a person recorded goes with their account.</summary>
public interface IAccountContentReader
{
    Task<AccountContentSummary> SummarizeAsync(Guid accountId, CancellationToken cancellationToken);
}

/// <param name="Memories">Memories the person is the author of.</param>
/// <param name="Photos">Photos in those memories.</param>
public sealed record AccountContentSummary(int Memories, int Photos);
