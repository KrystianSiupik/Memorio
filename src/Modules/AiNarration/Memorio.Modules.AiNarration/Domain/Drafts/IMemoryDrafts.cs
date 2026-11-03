namespace Memorio.Modules.AiNarration.Domain.Drafts;

internal interface IMemoryDrafts
{
    Task<MemoryDraft?> FindAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Stores the draft, or finds the one already under its id.</summary>
    Task<MemoryDraft> AddOrFindAsync(MemoryDraft draft, CancellationToken cancellationToken);
}
