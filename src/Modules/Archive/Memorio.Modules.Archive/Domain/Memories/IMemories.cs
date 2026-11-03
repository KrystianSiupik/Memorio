namespace Memorio.Modules.Archive.Domain.Memories;

internal interface IMemories
{
    /// <summary>Every memory the account is the author of, with its photos.</summary>
    Task<IReadOnlyList<Memory>> FindByAuthorAsync(Guid accountId, CancellationToken cancellationToken);

    void Remove(Memory memory);

    Task<AppliedAccountEvent?> FindAppliedAsync(Guid accountId, CancellationToken cancellationToken);

    void Add(AppliedAccountEvent applied);

    Task SaveAsync(CancellationToken cancellationToken);
}
