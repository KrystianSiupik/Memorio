using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Limits.Domain.Pools;

internal static class PoolErrors
{
    /// <summary>E151.</summary>
    public static readonly Error PoolFull = Error.Conflict("Limits.PoolFull", "There is no room left in the pool for this.");

    public static readonly Error InvalidUnits = Error.Validation("Limits.InvalidUnits", "An operation takes more than zero bytes.");
}
