namespace Memorio.Modules.Archive.Domain.Memories;

internal enum MemoryState
{
    Active,
    Trashed,

    /// <summary>It would be gone for good, but an order of an authority holds its author's data (C114): nobody sees it, and it goes once the hold ends.</summary>
    Erasing,
}

internal enum TrashReason
{
    Person,
    Trimmed,
}
