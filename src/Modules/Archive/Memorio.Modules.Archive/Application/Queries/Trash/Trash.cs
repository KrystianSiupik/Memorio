namespace Memorio.Modules.Archive.Application.Queries.Trash;

/// <summary>R15: the person's own content in the trash whose window has not ended.</summary>
internal interface ITrashQuery
{
    Task<IReadOnlyList<TrashedMemory>> GetAsync(Guid accountId, CancellationToken cancellationToken);
}

/// <param name="DaysLeft">Whole days, rounded up: "1" means less than a day is left.</param>
internal sealed record TrashedMemory(Guid Id, string PlaceName, string DateFrom, string? Description, int Photos, DateTimeOffset TrashedAt, int DaysLeft);
