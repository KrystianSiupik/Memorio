using Memorio.Modules.Plans.Domain.Entitlements;

namespace Memorio.Modules.Plans.Domain.Cycle;

/// <summary>What a pool may hold.</summary>
internal sealed record PoolTerms(long LimitBytes, int AiLimit);

/// <summary>The numbers of the plans (D-080, proposed until tested with users; D-261 for the free album).</summary>
internal static class PlanTerms
{
    public const int FreeAlbumSeats = 5;
    public const int PaidAlbumSeats = 10;

    private const long GigaByte = 1024L * 1024 * 1024;

    public static readonly PoolTerms Free = new(2 * GigaByte, 0);

    public static PoolTerms Paid(EntitledResourceKind kind) =>
        kind == EntitledResourceKind.Album ? new PoolTerms(150 * GigaByte, 60) : new PoolTerms(50 * GigaByte, 20);

    /// <summary>An album takes nothing new at all; an account takes nothing new over the free limit (S2:B1, S2:C7).</summary>
    public static PoolTerms ReadOnly(EntitledResourceKind kind) =>
        kind == EntitledResourceKind.Album ? new PoolTerms(0, 0) : Free;
}
