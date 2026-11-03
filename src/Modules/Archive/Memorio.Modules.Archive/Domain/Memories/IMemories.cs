namespace Memorio.Modules.Archive.Domain.Memories;

internal interface IMemories
{
    /// <summary>Every memory the account is the author of, with its photos.</summary>
    Task<IReadOnlyList<Memory>> FindByAuthorAsync(Guid accountId, CancellationToken cancellationToken);

    /// <summary>Stores the memory, or finds the one already under its id (the id comes from the client, D-118).</summary>
    Task<Memory> AddOrFindAsync(Memory memory, CancellationToken cancellationToken);

    void Remove(Memory memory);

    Task<AppliedAccountEvent?> FindAppliedAsync(Guid accountId, CancellationToken cancellationToken);

    void Add(AppliedAccountEvent applied);

    /// <summary>
    /// Stores the changes with the events of the changed memories, in one transaction. False when another
    /// decision about a memory got there first: nothing was stored.
    /// </summary>
    Task<bool> SaveAsync(CancellationToken cancellationToken);
}
