namespace Memorio.Modules.Archive.Application.Queries.MyMemories;

/// <summary>The person's own active memories, newest first: what they can bring to an album.</summary>
internal interface IMyMemoriesQuery
{
    Task<IReadOnlyList<MyMemory>> GetAsync(Guid accountId, CancellationToken cancellationToken);
}

internal sealed record MyMemory(Guid Id, string PlaceName, string DateFrom);
