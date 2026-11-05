using Memorio.Modules.Album.Domain.Albums;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Limits.Contracts;
using Memorio.Modules.Plans.Contracts;
using Memorio.SharedKernel.Messaging;

namespace Memorio.Modules.Album.Application.Policies;

/// <summary>
/// P33 for an album (D-290): what is over the limit leaves the album, and every author keeps their memory and pays for
/// it again. What leaves is written down first, in the transaction of the sequence, so a retry finishes the same list
/// even after Archive already let some of it go.
/// </summary>
internal sealed class TrimAlbumWhenOrdered(IAlbums albums, IAlbumContent content, IAlbumPool pool, TimeProvider clock) : IIntegrationEventPolicy<TrimOrderedEvent>
{
    public async Task HandleAsync(TrimOrderedEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (!integrationEvent.IsAlbum)
        {
            return;
        }

        var id = new AlbumId(integrationEvent.ResourceId);
        var applied = await albums.FindAppliedPlanEventAsync(id, cancellationToken);
        if (applied?.Sequence > integrationEvent.Sequence)
        {
            return;
        }

        if (applied?.Sequence != integrationEvent.Sequence)
        {
            var now = clock.GetUtcNow();
            foreach (var memoryId in await content.ChooseToTrimAsync(id.Value, integrationEvent.KeptMemoryIds, integrationEvent.LimitBytes, cancellationToken))
            {
                if (await albums.FindMemoryAsync(id, memoryId, cancellationToken) is { } memory)
                {
                    albums.Add(TrimmedAlbumMemory.Create(id, memoryId, memory.AuthorAccountId, now));
                }
            }

            albums.RecordApplied(applied, id, integrationEvent.Sequence);
            await SaveAsync(cancellationToken);
        }

        foreach (var trimmed in await albums.FindTrimmedAsync(id, cancellationToken))
        {
            if (trimmed.HasLeft)
            {
                continue;
            }

            await content.UnlinkAsync(trimmed.MemoryId, id.Value, cancellationToken);
            await pool.ReleaseAsync(PoolKeys.For(id.Value, trimmed.MemoryId), cancellationToken);
            if (await albums.FindMemoryAsync(id, trimmed.MemoryId, cancellationToken) is { } memory)
            {
                albums.Remove(memory);
            }

            trimmed.MarkLeft();
            await SaveAsync(cancellationToken);
        }
    }

    private async Task SaveAsync(CancellationToken cancellationToken)
    {
        if (!await albums.SaveAsync(cancellationToken))
        {
            throw new InvalidOperationException("Another decision about this album got there first; the retry reads it again.");
        }
    }
}

/// <summary>
/// P44 for an album: paid again within 30 days, what left comes back to the album, also above the limit, because the
/// event about the new limit may be behind this one. Only what its author may still bring. Tags and order stay gone.
/// </summary>
internal sealed class RestoreTrimmedWhenPlanActive(IAlbums albums, IAlbumContent content, IAlbumPool pool, TimeProvider clock) : IIntegrationEventPolicy<PlanActivatedEvent>
{
    public static readonly TimeSpan Window = TimeSpan.FromDays(30);

    public async Task HandleAsync(PlanActivatedEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (!integrationEvent.IsAlbum)
        {
            return;
        }

        var id = new AlbumId(integrationEvent.ResourceId);
        var applied = await albums.FindAppliedPlanEventAsync(id, cancellationToken);
        if (applied?.Sequence >= integrationEvent.Sequence)
        {
            return;
        }

        var album = await albums.FindAsync(id, cancellationToken);
        var now = clock.GetUtcNow();
        foreach (var trimmed in await albums.FindTrimmedAsync(id, cancellationToken))
        {
            if (album?.HasMember(trimmed.AuthorAccountId) == true
                && now < trimmed.TrimmedAt + Window
                && await content.DescribeAsync(trimmed.MemoryId, cancellationToken) is { } facts
                && await albums.FindMemoryAsync(id, trimmed.MemoryId, cancellationToken) is null)
            {
                await pool.ReserveOverLimitAsync(id.Value, PoolKeys.For(id.Value, trimmed.MemoryId), facts.SizeBytes, cancellationToken);
                await content.LinkAsync(trimmed.MemoryId, id.Value, cancellationToken);
                albums.Add(AlbumMemory.Create(id, trimmed.MemoryId, trimmed.AuthorAccountId, now));
            }

            albums.Remove(trimmed);
        }

        albums.RecordApplied(applied, id, integrationEvent.Sequence);
        if (!await albums.SaveAsync(cancellationToken))
        {
            throw new InvalidOperationException("Another decision about this album got there first; the retry reads it again.");
        }
    }
}
