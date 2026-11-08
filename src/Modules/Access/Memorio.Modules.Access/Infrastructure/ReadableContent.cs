using Memorio.Modules.Access.Contracts;

namespace Memorio.Modules.Access.Infrastructure;

/// <summary>The read port built on the one answer of ADR 0011 (D-035): nothing decided here that the reader does not decide.</summary>
internal sealed class ReadableContent(IAccessReader access) : IReadableContent
{
    public async Task<IReadOnlyCollection<Guid>> ReadableAsync(Guid personId, Guid albumId, IReadOnlyCollection<Guid> memoryIds, CancellationToken cancellationToken)
    {
        if (memoryIds.Count == 0 || !await access.CanSeeAlbumAsync(personId, albumId, cancellationToken))
        {
            return [];
        }

        var hidden = await access.HiddenFromAsync(personId, [albumId, .. memoryIds], cancellationToken);
        return hidden.Contains(albumId) ? [] : [.. memoryIds.Distinct().Where(id => !hidden.Contains(id))];
    }
}
