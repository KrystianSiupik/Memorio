using Dapper;
using Memorio.Modules.Plans.Contracts;
using Memorio.Modules.Plans.Domain.Cycle;
using Memorio.Modules.Plans.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Memorio.Modules.Plans.Infrastructure;

/// <summary>5 people without a paid plan (D-261), 10 with it (D-080), and no new seat while read-only (S2:B1).</summary>
internal sealed class AlbumSeatLimit(PlansDbContext database) : IAlbumSeatLimit
{
    public async Task<int> SeatsOfAsync(Guid albumId, CancellationToken cancellationToken) =>
        await ModeOfAsync(albumId, cancellationToken) switch
        {
            nameof(PlanMode.Active) => PlanTerms.PaidAlbumSeats,
            nameof(PlanMode.ReadOnly) => 0,
            _ => PlanTerms.FreeAlbumSeats,
        };

    public async Task<bool> IsReadOnlyAsync(Guid albumId, CancellationToken cancellationToken) =>
        await ModeOfAsync(albumId, cancellationToken) == nameof(PlanMode.ReadOnly);

    private Task<string?> ModeOfAsync(Guid albumId, CancellationToken cancellationToken) =>
        database.Database.GetDbConnection().ExecuteScalarAsync<string?>(new CommandDefinition(
            "SELECT mode FROM plans.plan_cycles WHERE resource_kind = 'Album' AND resource_id = @albumId",
            new { albumId },
            cancellationToken: cancellationToken));
}
