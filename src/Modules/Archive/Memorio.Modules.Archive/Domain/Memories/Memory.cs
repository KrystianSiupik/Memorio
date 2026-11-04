using Memorio.SharedKernel.Domain;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Archive.Domain.Memories;

/// <summary>
/// What a person recorded: where, when and what. The place is a snapshot taken when choosing it, so the
/// map needs no other module. One author: an album only points at it, and only the author adds to it (D-248).
/// </summary>
internal sealed class Memory : AggregateRoot<MemoryId>
{
    public const int MaximumDescriptionLength = 20_000;

    public static readonly TimeSpan TrashWindow = TimeSpan.FromDays(30);

    private readonly List<Photo> _photos = [];

    private Memory(MemoryId id, Guid authorAccountId, DateTimeOffset createdAt)
        : base(id)
    {
        AuthorAccountId = authorAccountId;
        CreatedAt = createdAt;
    }

    public Guid AuthorAccountId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public string PlaceReference { get; private set; } = "";

    public string PlaceName { get; private set; } = "";

    public double Latitude { get; private set; }

    public double Longitude { get; private set; }

    public DatePrecision DatePrecision { get; private set; }

    public DateOnly DateFrom { get; private set; }

    public DateOnly DateTo { get; private set; }

    public IReadOnlyCollection<Photo> Photos => _photos.AsReadOnly();

    public MemoryState State { get; private set; } = MemoryState.Active;

    public TrashReason? TrashReason { get; private set; }

    public DateTimeOffset? TrashedAt { get; private set; }

    public DateTimeOffset? WindowEndsAt { get; private set; }

    /// <summary>Grows with every move to or from the trash; ties a deadline to its window (D-013).</summary>
    public int Sequence { get; private set; } = 1;

    public string? Description { get; private set; }

    /// <summary>With <see cref="DescriptionAiPromptVersion"/>: set when the text started as an AI draft (SD9).</summary>
    public string? DescriptionAiModel { get; private set; }

    public string? DescriptionAiPromptVersion { get; private set; }

    public static Memory Create(MemoryId id, Guid authorAccountId, MemoryPlace place, MemoryDate date, DateTimeOffset now)
    {
        var memory = new Memory(id, authorAccountId, now)
        {
            PlaceReference = place.Reference,
            PlaceName = place.Name,
            Latitude = place.Latitude,
            Longitude = place.Longitude,
            DatePrecision = date.Precision,
            DateFrom = date.From,
            DateTo = date.To,
        };
        memory.Raise(new MemoryCreated(Guid.CreateVersion7(), now, id, authorAccountId));
        return memory;
    }

    /// <summary>
    /// The person stands behind this text. A text that started as an AI draft keeps the marker, also after
    /// edits: the person may change a word, and it is still written with AI (AI Act art. 50, K9).
    /// </summary>
    public Result ApproveDescription(string? text, MemoryAiOrigin? aiOrigin, DateTimeOffset now)
    {
        var trimmed = text?.Trim();
        if (string.IsNullOrEmpty(trimmed) || trimmed.Length > MaximumDescriptionLength)
        {
            return MemoryErrors.InvalidDescription;
        }

        Description = trimmed;
        DescriptionAiModel = aiOrigin?.Model;
        DescriptionAiPromptVersion = aiOrigin?.PromptVersion;
        Raise(new DescriptionApproved(Guid.CreateVersion7(), now, Id));
        return Result.Success();
    }

    /// <summary>E20: 30 days in the trash. Asking again changes nothing.</summary>
    public void MoveToTrash(DateTimeOffset now)
    {
        if (State == MemoryState.Trashed)
        {
            return;
        }

        State = MemoryState.Trashed;
        TrashReason = Memories.TrashReason.Person;
        TrashedAt = now;
        WindowEndsAt = now + TrashWindow;
        Sequence++;
        Raise(new MemoryMovedToTrash(Guid.CreateVersion7(), now, Id, AuthorAccountId, Sequence, WindowEndsAt.Value));
    }

    public bool HasPhoto(Guid photoId) => _photos.Exists(photo => photo.Id == photoId);

    public void AddPhoto(Guid photoId, string storageKey, long sizeBytes, DateTimeOffset now, Guid? albumId = null)
    {
        _photos.Add(Photo.Create(photoId, storageKey, sizeBytes, albumId));
        Raise(new PhotoAdded(Guid.CreateVersion7(), now, Id, photoId));
    }

    /// <summary>
    /// Only before the window ends: at its end the content goes, even when the deadline is still on its way.
    /// What was trimmed comes back only with a paid plan (HS-022).
    /// </summary>
    public Result Restore(DateTimeOffset now, bool planPaid)
    {
        if (State != MemoryState.Trashed || now >= WindowEndsAt)
        {
            return MemoryErrors.AlreadyDecided;
        }

        if (TrashReason == Memories.TrashReason.Trimmed && !planPaid)
        {
            return MemoryErrors.PlanRequired;
        }

        State = MemoryState.Active;
        TrashReason = null;
        TrashedAt = null;
        WindowEndsAt = null;
        Sequence++;
        Raise(new MemoryRestored(Guid.CreateVersion7(), now, Id, AuthorAccountId, Sequence));
        return Result.Success();
    }

    /// <summary>The trim of an archive over its limit moves material here (HS-022); called by MEM-E7-ARCHIWUM-01.</summary>
    public void MoveToTrashByTrimming(DateTimeOffset now)
    {
        MoveToTrash(now);
        TrashReason = Memories.TrashReason.Trimmed;
    }

    /// <summary>The end of the trash window: only the window it was meant for, only when still open and only after its end.</summary>
    public Result CanLeaveTrash(int sequence, DateTimeOffset at)
    {
        if (State != MemoryState.Trashed || sequence != Sequence)
        {
            return MemoryErrors.AlreadyDecided;
        }

        return at < WindowEndsAt ? MemoryErrors.TrashWindowOpen : Result.Success();
    }

    /// <summary>E22 and E23: one author, so the last content goes with the memory (D-221).</summary>
    public void DeleteDefinitively(DateTimeOffset now)
    {
        Raise(new ContentDeletedDefinitively(Guid.CreateVersion7(), now, Id, AuthorAccountId, Sequence));
        Raise(new MemoryCeasedToExist(Guid.CreateVersion7(), now, Id));
    }
}

/// <summary>Which AI wrote the draft a description started from.</summary>
internal sealed record MemoryAiOrigin(string Model, string PromptVersion);

/// <summary>What a memory keeps of its place.</summary>
internal sealed record MemoryPlace(string Reference, string Name, double Latitude, double Longitude);
