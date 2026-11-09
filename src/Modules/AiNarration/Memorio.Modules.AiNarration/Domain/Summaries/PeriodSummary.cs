using Memorio.Modules.AiNarration.Domain.AiTexts;
using Memorio.SharedKernel.Domain;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.AiNarration.Domain.Summaries;

/// <summary>
/// C82–C84: the AI's story of a period of an album (S1 §9). Made only of the listed facts: every sentence points to the
/// facts it tells (S2:A2), sentences that point nowhere are dropped before anyone sees them. It has a date and the
/// material it was made from (C5: who joined later does not change it), says what was left out, and once saved it lives
/// only while its sources may (B2): invalidated, its text is gone for good.
/// </summary>
internal sealed class PeriodSummary : AggregateRoot<Guid>
{
    private readonly List<SummarySentence> _sentences = [];
    private readonly List<SummarySource> _sources = [];

    private PeriodSummary(Guid id, Guid accountId, Guid albumId, DateOnly from, DateOnly to)
        : base(id)
    {
        AccountId = accountId;
        AlbumId = albumId;
        From = from;
        To = to;
    }

    /// <summary>Who asked for it and alone may save it.</summary>
    public Guid AccountId { get; private set; }

    public Guid AlbumId { get; private set; }

    public DateOnly From { get; private set; }

    public DateOnly To { get; private set; }

    public string Model { get; private set; } = "";

    public string PromptVersion { get; private set; } = "";

    public DateTimeOffset GeneratedAt { get; private set; }

    /// <summary>When the materials were read (AG9, D-023).</summary>
    public DateTimeOffset MaterialsReadAt { get; private set; }

    public int LeftOutMemories { get; private set; }

    public int LeftOutPeople { get; private set; }

    /// <summary>Sentences of the AI that pointed to no given fact.</summary>
    public int DroppedSentences { get; private set; }

    public SummaryState State { get; private set; } = SummaryState.Draft;

    public DateTimeOffset? SavedAt { get; private set; }

    public DateTimeOffset? AddedToAlbumAt { get; private set; }

    public DateTimeOffset? InvalidatedAt { get; private set; }

    public string? InvalidReason { get; private set; }

    public int Sequence { get; private set; } = 1;

    public IReadOnlyList<SummarySentence> Sentences => _sentences;

    /// <summary>Every memory given to the AI, with its author: what the artifact was made from.</summary>
    public IReadOnlyList<SummarySource> Sources => _sources;

    /// <summary>C82, E82: from the facts and the AI's sentences. Null sentences were all dropped: E83.</summary>
    public static Result<PeriodSummary> Generate(
        Guid id,
        Guid accountId,
        Guid albumId,
        DateOnly from,
        DateOnly to,
        IReadOnlyList<SummarySource> sources,
        IReadOnlyList<SummarySentence> sentences,
        int droppedSentences,
        int leftOutMemories,
        int leftOutPeople,
        AiGeneratedText text,
        DateTimeOffset materialsReadAt)
    {
        if (sentences.Count == 0)
        {
            return SummaryErrors.NothingTold;
        }

        var summary = new PeriodSummary(id, accountId, albumId, from, to)
        {
            Model = text.Model,
            PromptVersion = text.PromptVersion,
            GeneratedAt = text.GeneratedAt,
            MaterialsReadAt = materialsReadAt,
            LeftOutMemories = leftOutMemories,
            LeftOutPeople = leftOutPeople,
            DroppedSentences = droppedSentences,
        };
        summary._sentences.AddRange(sentences);
        summary._sources.AddRange(sources);
        summary.Raise(new SummaryGenerated(Guid.CreateVersion7(), text.GeneratedAt, id, accountId, albumId, summary.Sequence));
        return summary;
    }

    /// <summary>The whole story, the sentences one after another.</summary>
    public string Text => string.Join(" ", _sentences.Select(sentence => sentence.Text));

    /// <summary>C83, E84: the person approved it as it is (S2:A2: approval before saving).</summary>
    public Result Save(Guid accountId, DateTimeOffset now)
    {
        if (accountId != AccountId)
        {
            return SummaryErrors.NotYours;
        }

        if (State == SummaryState.Invalidated)
        {
            return SummaryErrors.Invalidated;
        }

        if (State != SummaryState.Draft)
        {
            return Result.Success();
        }

        State = SummaryState.Saved;
        SavedAt = now;
        Sequence++;
        Raise(new SummarySaved(Guid.CreateVersion7(), now, Id, AccountId, AlbumId, Sequence));
        return Result.Success();
    }

    /// <summary>C84, E85: a saved summary shown to the album's participants.</summary>
    public Result AddToAlbum(Guid accountId, DateTimeOffset now)
    {
        if (accountId != AccountId)
        {
            return SummaryErrors.NotYours;
        }

        if (State == SummaryState.Invalidated)
        {
            return SummaryErrors.Invalidated;
        }

        if (State == SummaryState.Draft)
        {
            return SummaryErrors.NotSaved;
        }

        if (AddedToAlbumAt is not null)
        {
            return Result.Success();
        }

        AddedToAlbumAt = now;
        Sequence++;
        Raise(new SummaryAddedToAlbum(Guid.CreateVersion7(), now, Id, AccountId, AlbumId, Sequence));
        return Result.Success();
    }

    /// <summary>E61 from Provenance: its text goes for good (B2: it does not come back).</summary>
    public bool Invalidate(string reason, DateTimeOffset now)
    {
        if (State == SummaryState.Invalidated)
        {
            return false;
        }

        State = SummaryState.Invalidated;
        InvalidatedAt = now;
        InvalidReason = reason;
        _sentences.Clear();
        Sequence++;
        return true;
    }
}

/// <param name="MemoryIds">The memories the sentence tells of (the facts it points to).</param>
internal sealed record SummarySentence(string Text, IReadOnlyList<Guid> MemoryIds);

internal sealed record SummarySource(Guid MemoryId, Guid AuthorAccountId);

internal enum SummaryState
{
    Draft,
    Saved,
    Invalidated,
}

/// <summary>E82.</summary>
internal sealed record SummaryGenerated(Guid EventId, DateTimeOffset OccurredAt, Guid SummaryId, Guid AccountId, Guid AlbumId, int Sequence) : IDomainEvent;

/// <summary>E84.</summary>
internal sealed record SummarySaved(Guid EventId, DateTimeOffset OccurredAt, Guid SummaryId, Guid AccountId, Guid AlbumId, int Sequence) : IDomainEvent;

/// <summary>E85.</summary>
internal sealed record SummaryAddedToAlbum(Guid EventId, DateTimeOffset OccurredAt, Guid SummaryId, Guid AccountId, Guid AlbumId, int Sequence) : IDomainEvent;

internal static class SummaryErrors
{
    public static readonly Error InvalidPeriod = Error.Validation("AiNarration.InvalidPeriod", "A summary needs an album and a period whose start is not after its end, of at most 10 years.");

    public static readonly Error NotMember = Error.Forbidden("AiNarration.NotMember", "Only a participant of the album asks for its summary.");

    public static readonly Error ConsentRequired = Error.Forbidden("AiNarration.ConsentRequired", "Give the AI consent for this album first.");

    public static readonly Error NoMaterials = Error.Validation("AiNarration.NoMaterials", "There is nothing in this period that the AI may use.");

    public static readonly Error NothingTold = Error.Failure("AiNarration.NothingTold", "The AI wrote nothing that the memories support; try again.");

    public static readonly Error NotFound = Error.NotFound("AiNarration.SummaryNotFound", "No such summary.");

    public static readonly Error NotYours = Error.Forbidden("AiNarration.NotYours", "Only the person who asked for the summary decides on it.");

    public static readonly Error NotSaved = Error.Conflict("AiNarration.NotSaved", "Save the summary before adding it to the album.");

    public static readonly Error Invalidated = Error.Conflict("AiNarration.Invalidated", "This summary is no longer valid; ask for a new one.");

    public static readonly Error MaterialsChanged = Error.Conflict("AiNarration.MaterialsChanged", "Some memories it tells of may no longer be used; ask for a new summary.");
}
