using System.Globalization;
using System.Text.RegularExpressions;
using Memorio.Modules.Plans.Application.Commands.RecordEntitlement;
using Memorio.Modules.Plans.Application.Entitlements;
using Memorio.Modules.Plans.Domain.Entitlements;
using Memorio.SharedKernel.Results;
using Memorio.Testing;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Memorio.Modules.Plans.Tests;

/// <summary>
/// MEM-E0-PLAT-15 on the real database: payment sources write lines to the ledger, and what the plan cycle reads
/// (<see cref="PlanEntitlements"/>) comes from the ledger only (D-078). The manual source is deploy/plans/*.sql
/// itself, run the way psql runs it (<c>:'name'</c> are the script's values), so the script's statements are
/// what is tested.
/// </summary>
public sealed partial class EntitlementLedgerTests(PlansDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task A_manual_grant_is_what_the_plan_cycle_reads_and_ending_it_ends_the_plan()
    {
        var album = new EntitledResource(EntitledResourceKind.Album, Guid.NewGuid());
        var until = new DateTimeOffset(2027, 3, 31, 23, 59, 59, TimeSpan.Zero);

        var granted = Assert.Single(await RunScriptSqlAsync("grant.sql", album, until, "tester of the album plan"));
        Assert.Contains("tester of the album plan", granted, StringComparison.Ordinal);
        Assert.Equal(until, await EntitledUntilAsync(album));

        var ended = DateTimeOffset.UtcNow;
        await RunScriptSqlAsync("grant.sql", album, ended, "tester left");

        var entitledUntil = await EntitledUntilAsync(album);
        Assert.True(entitledUntil <= DateTimeOffset.UtcNow, $"Still paid until {entitledUntil} after the manual end.");
        Assert.Equal(2, (await RunScriptSqlAsync("list.sql", album)).Count);
    }

    [Fact]
    public async Task A_manual_line_without_a_reason_is_refused_by_the_database()
    {
        var account = new EntitledResource(EntitledResourceKind.Account, Guid.NewGuid());

        var refused = await Assert.ThrowsAsync<PostgresException>(
            () => RunScriptSqlAsync("grant.sql", account, DateTimeOffset.UtcNow.AddMonths(1), " "));

        Assert.Equal(PostgresErrorCodes.CheckViolation, refused.SqlState);
        Assert.Null(await EntitledUntilAsync(account));
    }

    [Fact]
    public async Task The_same_notification_recorded_twice_stays_one_line()
    {
        var account = new EntitledResource(EntitledResourceKind.Account, Guid.NewGuid());
        var notification = new RecordEntitlement("test-store", "notification-1", account, DateTimeOffset.UtcNow.AddMonths(1), DateTimeOffset.UtcNow);

        Assert.True((await RecordAsync(notification)).Value);
        Assert.False((await RecordAsync(notification)).Value);

        Assert.Single(await RunScriptSqlAsync("list.sql", account));
    }

    [Fact]
    public async Task A_late_notification_of_a_source_does_not_undo_its_newer_one()
    {
        var account = new EntitledResource(EntitledResourceKind.Account, Guid.NewGuid());
        var now = DateTimeOffset.UtcNow;

        await RecordAsync(new RecordEntitlement("test-store", "renewal", account, now.AddMonths(2), now.AddMinutes(-1)));
        await RecordAsync(new RecordEntitlement("test-store", "first-purchase", account, now.AddMonths(1), now.AddMonths(-1)));

        Assert.Equal(now.AddMonths(2), (await EntitledUntilAsync(account))!.Value, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public async Task An_invalid_notification_is_refused_and_not_written()
    {
        var account = new EntitledResource(EntitledResourceKind.Account, Guid.NewGuid());

        var result = await RecordAsync(new RecordEntitlement(EntitlementSources.Manual, "m1", account, DateTimeOffset.UtcNow.AddDays(1), DateTimeOffset.UtcNow));

        Assert.Equal(EntitlementErrors.ReasonRequired, result.Error);
        Assert.Null(await EntitledUntilAsync(account));
    }

    private async Task<Result<bool>> RecordAsync(RecordEntitlement command)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<RecordEntitlementHandler>().HandleAsync(command, Cancellation);
    }

    private async Task<DateTimeOffset?> EntitledUntilAsync(EntitledResource resource)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<PlanEntitlements>().EntitledUntilAsync(resource, Cancellation);
    }

    /// <summary>
    /// A SQL file of deploy/plans as the owner's psql session would run it (it switches to the role itself),
    /// with the values entitlements.sh passes in place of <c>:'name'</c>; every row, columns joined by "|".
    /// </summary>
    private async Task<List<string>> RunScriptSqlAsync(string file, EntitledResource resource, DateTimeOffset? until = null, string? note = null)
    {
        var sql = await File.ReadAllTextAsync(Path.Combine(PostgresDatabase.RepositoryRoot(), "deploy", "plans", file), Cancellation);
        var values = new Dictionary<string, string?>
        {
            ["kind"] = resource.Kind.ToString(),
            ["id"] = resource.Id.ToString(),
            ["until"] = until?.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
            ["note"] = note,
        };

        await using var connection = await database.OpenAsOwnerAsync();
        await using var command = new NpgsqlCommand(ScriptVariable().Replace(sql, "@$1"), connection);
        foreach (var (name, value) in values)
        {
            command.Parameters.AddWithValue(name, value ?? "");
        }

        var rows = new List<string>();
        await using var reader = await command.ExecuteReaderAsync(Cancellation);
        do
        {
            while (await reader.ReadAsync(Cancellation))
            {
                rows.Add(string.Join('|', Enumerable.Range(0, reader.FieldCount).Select(i => reader.IsDBNull(i) ? "" : Convert.ToString(reader.GetValue(i), CultureInfo.InvariantCulture))));
            }
        }
        while (await reader.NextResultAsync(Cancellation));

        return rows;
    }

    [GeneratedRegex(@":'(\w+)'")]
    private static partial Regex ScriptVariable();
}
