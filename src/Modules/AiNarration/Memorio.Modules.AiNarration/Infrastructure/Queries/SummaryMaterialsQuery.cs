using Memorio.Modules.Access.Contracts;
using Memorio.Modules.AiNarration.Application.Queries.SummaryMaterials;
using Memorio.Modules.AiNarration.Infrastructure.Persistence;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Archive.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.AiNarration.Infrastructure.Queries;

/// <summary>R8 as a projection without rules of its own: Access decides what is readable, the consents what may go to the AI.</summary>
internal sealed class SummaryMaterialsQuery(
    IAlbumMemories album,
    IReadableContent access,
    IMemoryFacts archive,
    AiNarrationDbContext database,
    TimeProvider clock) : ISummaryMaterialsQuery
{
    public async Task<SummaryMaterials> ForAsync(Guid accountId, Guid albumId, DateOnly from, DateOnly to, CancellationToken cancellationToken)
    {
        var readAt = clock.GetUtcNow();
        var memories = await album.MemoriesOfAsync(albumId, cancellationToken);
        var readable = await access.ReadableAsync(accountId, albumId, [.. memories.Select(memory => memory.MemoryId)], cancellationToken);
        var facts = (await archive.FactsAsync(readable, cancellationToken))
            .Where(fact => fact.DateFrom <= to && fact.DateTo >= from)
            .ToList();

        var authors = facts.Select(fact => fact.AuthorAccountId).Distinct().ToArray();
        var consenting = await database.AiConsents.AsNoTracking()
            .Where(consent => consent.AlbumId == albumId && consent.IsGiven && authors.Contains(consent.AccountId))
            .Select(consent => consent.AccountId)
            .ToListAsync(cancellationToken);

        var used = facts.Where(fact => consenting.Contains(fact.AuthorAccountId)).ToList();
        var leftOut = facts.Where(fact => !consenting.Contains(fact.AuthorAccountId)).ToList();
        return new SummaryMaterials(
            [.. used.Select(fact => new MaterialFact(fact.MemoryId, fact.AuthorAccountId, fact.PlaceName, fact.DatePrecision, fact.DateFrom, fact.DateTo, fact.Time, fact.Description))],
            leftOut.Count,
            leftOut.Select(fact => fact.AuthorAccountId).Distinct().Count(),
            readAt);
    }
}
