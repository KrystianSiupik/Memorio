namespace Memorio.Modules.Access.Contracts;

/// <summary>
/// Read port for AiNarration (C82): of these memories of an album, which the person may read now. The album must be
/// visible to them and neither it nor the memory hidden by Moderation (E161). Asked at every use, never cached (C9).
/// </summary>
public interface IReadableContent
{
    Task<IReadOnlyCollection<Guid>> ReadableAsync(Guid personId, Guid albumId, IReadOnlyCollection<Guid> memoryIds, CancellationToken cancellationToken);
}
