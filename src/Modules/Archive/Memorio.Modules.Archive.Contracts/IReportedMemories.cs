namespace Memorio.Modules.Archive.Contracts;

/// <summary>Read port for Moderation (C110): a memory can be reported only by someone who sees it now.</summary>
public interface IReportedMemories
{
    /// <summary>Its author, or null when the person does not see it: not there and not visible look the same.</summary>
    Task<Guid?> AuthorSeenByAsync(Guid memoryId, Guid personId, CancellationToken cancellationToken);
}
