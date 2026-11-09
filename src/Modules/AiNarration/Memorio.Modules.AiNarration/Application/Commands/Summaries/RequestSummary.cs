using System.Globalization;
using System.Text;
using Memorio.Modules.AiNarration.Application.AiTexts;
using Memorio.Modules.AiNarration.Application.Ports;
using Memorio.Modules.AiNarration.Application.Queries.SummaryMaterials;
using Memorio.Modules.AiNarration.Domain.Consents;
using Memorio.Modules.AiNarration.Domain.Pseudonymization;
using Memorio.Modules.AiNarration.Domain.Summaries;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Limits.Contracts;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.AiNarration.Application.Commands.Summaries;

/// <summary>C82: a participant asks for the story of a period of an album. The id comes from the client: asking again gets the same summary.</summary>
internal sealed record RequestSummary(Guid Id, Guid AccountId, Guid AlbumId, DateOnly From, DateOnly To);

/// <summary>
/// Only the materials of R8 go out, as numbered facts without names or photos (S2:C5); the allowance is used before the
/// provider is called and stays used on its failure, and the same id again costs nothing (D-226).
/// </summary>
internal sealed class RequestSummaryHandler(
    IPeriodSummaries summaries,
    IAiConsents consents,
    IAlbumMembership membership,
    ISummaryMaterialsQuery materials,
    IAiAllowance allowance,
    AiTextService writer,
    IPromptCatalog prompts)
{
    public async Task<Result<PeriodSummary>> HandleAsync(RequestSummary command, CancellationToken cancellationToken)
    {
        if (command.AlbumId == Guid.Empty || command.From > command.To || command.To.DayNumber - command.From.DayNumber > 3660)
        {
            return SummaryErrors.InvalidPeriod;
        }

        if (await summaries.FindAsync(command.Id, cancellationToken) is { } existing)
        {
            return existing.AccountId == command.AccountId ? existing : SummaryErrors.NotYours;
        }

        if (!await membership.IsMemberAsync(command.AccountId, command.AlbumId, cancellationToken))
        {
            return SummaryErrors.NotMember;
        }

        if (await consents.FindAsync(command.AccountId, command.AlbumId, cancellationToken) is not { IsGiven: true })
        {
            return SummaryErrors.ConsentRequired;
        }

        var found = await materials.ForAsync(command.AccountId, command.AlbumId, command.From, command.To, cancellationToken);
        if (found.Facts.Count == 0)
        {
            return SummaryErrors.NoMaterials;
        }

        if (!await allowance.TryConsumeAsync(command.AccountId, command.Id, cancellationToken))
        {
            return Error.Conflict("AiNarration.LimitReached", "The AI allowance of this month is used up.");
        }

        var (material, labels, names) = Describe(found.Facts);
        var text = await writer.WriteAsync(prompts.PeriodSummary, material, names, cancellationToken);
        if (text.IsFailure)
        {
            return text.Error;
        }

        var (sentences, dropped) = CitedSentences.Read(text.Value.Content, labels);
        var summary = PeriodSummary.Generate(
            command.Id,
            command.AccountId,
            command.AlbumId,
            command.From,
            command.To,
            [.. found.Facts.Select(fact => new SummarySource(fact.MemoryId, fact.AuthorAccountId))],
            sentences,
            dropped,
            found.LeftOutMemories,
            found.LeftOutPeople,
            text.Value,
            found.ReadAt);
        if (summary.IsFailure)
        {
            return summary.Error;
        }

        return await summaries.AddOrFindAsync(summary.Value, cancellationToken);
    }

    /// <summary>
    /// The facts as the provider sees them: a label, the date, the kind of date, the description. Places are names and
    /// leave as tokens; the authors are "person A", "person B" (S2:C5: no names, no coordinates, no photos).
    /// </summary>
    internal static (string Material, IReadOnlyDictionary<string, Guid> Labels, IReadOnlyCollection<SensitiveName> Names) Describe(IReadOnlyList<MaterialFact> facts)
    {
        var material = new StringBuilder();
        var labels = new Dictionary<string, Guid>();
        var names = new List<SensitiveName>();
        var authors = new Dictionary<Guid, string>();
        foreach (var (fact, index) in facts.Select((fact, index) => (fact, index)))
        {
            var label = "F" + (index + 1).ToString(CultureInfo.InvariantCulture);
            labels[label] = fact.MemoryId;
            if (!authors.TryGetValue(fact.AuthorAccountId, out var author))
            {
                author = "person " + (char)('A' + (authors.Count % 26));
                authors[fact.AuthorAccountId] = author;
            }

            if (SensitiveName.Create(SensitiveNameKind.Place, fact.PlaceName) is { IsSuccess: true } place)
            {
                names.Add(place.Value);
            }

            var date = fact.DateFrom == fact.DateTo
                ? fact.DateFrom.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                : $"{fact.DateFrom:yyyy-MM-dd} to {fact.DateTo:yyyy-MM-dd}";
            material.Append(CultureInfo.InvariantCulture, $"{label}: {date} ({fact.DatePrecision})");
            if (fact.Time is not null)
            {
                material.Append(CultureInfo.InvariantCulture, $", {fact.Time}");
            }

            material.Append(CultureInfo.InvariantCulture, $". Place: {fact.PlaceName}. Recorded by {author}.");
            if (!string.IsNullOrWhiteSpace(fact.Description))
            {
                material.Append(CultureInfo.InvariantCulture, $" Description: {fact.Description.Trim()}");
            }

            material.AppendLine();
        }

        return (material.ToString(), labels, names.DistinctBy(name => name.Value).ToList());
    }
}
