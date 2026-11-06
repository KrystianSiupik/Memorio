using Memorio.Modules.Moderation.Application.Commands.Orders;
using Memorio.Modules.Moderation.Application.Policies;
using Memorio.Modules.Moderation.Contracts;
using Memorio.Modules.Moderation.Domain.Orders;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Memorio.Modules.Moderation.Tests;

/// <summary>Orders of authorities on a real database: a removal within the hour, data secured before it is disclosed (L11, L18).</summary>
[Collection(nameof(OperatorJobs))]
public sealed class OrderTests(ModerationTestDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    private async Task EnterAsync(Guid id, string kind, string? resourceKind = null, Guid? resource = null, Guid? account = null, bool confidential = false)
    {
        await using var connection = await database.OpenAsAsync(ModerationTestDatabase.Role);
        await using var command = new NpgsqlCommand(
            """
            INSERT INTO moderation.incoming_orders (id, kind, authority, reference, resource_kind, resource_id, account_id, confidential, received_by, received_at)
            VALUES (@id, @kind, 'Prokuratura Rejonowa', 'PR 1 Ds. 1/26, art. 218 kpk', @resourceKind, @resource, @account, @confidential, 'test', now())
            """,
            connection);
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("kind", kind);
        command.Parameters.AddWithValue("resourceKind", (object?)resourceKind ?? DBNull.Value);
        command.Parameters.AddWithValue("resource", (object?)resource ?? DBNull.Value);
        command.Parameters.AddWithValue("account", (object?)account ?? DBNull.Value);
        command.Parameters.AddWithValue("confidential", confidential);
        await command.ExecuteNonQueryAsync(Cancellation);
    }

    internal static async Task StepAsync(ModerationTestDatabase database, Guid order, string kind, string details = "Wspomnienia konta i zdjęcia, eksport z dnia.")
    {
        await using var connection = await database.OpenAsAsync(ModerationTestDatabase.Role);
        await using var command = new NpgsqlCommand(
            "INSERT INTO moderation.order_actions (id, order_id, kind, details, decided_by, decided_at) VALUES (gen_random_uuid(), @order, @kind, @details, 'test', now())",
            connection);
        command.Parameters.AddWithValue("order", order);
        command.Parameters.AddWithValue("kind", kind);
        command.Parameters.AddWithValue("details", details);
        await command.ExecuteNonQueryAsync(Cancellation);
    }

    private async Task<AuthorityOrder?> FindAsync(Guid id)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IOrders>().FindAsync(new OrderId(id), Cancellation);
    }

    private async Task<bool> HeldAsync(Guid account)
    {
        await using var scope = database.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ILegalHolds>().IsHeldAsync(account, Cancellation);
    }

    [Fact]
    public async Task A_removal_order_is_hidden_at_once_with_the_confirmation_of_Access_and_the_author_hears_it()
    {
        var (memory, author, _) = database.Neighbours.NewMemory();
        var id = Guid.CreateVersion7();
        await EnterAsync(id, nameof(OrderKind.RemoveContent), "Memory", memory);
        await ReportFlowTests.FireAsync(database);

        Assert.True(await ModerationTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(RemovalOrderReceivedEvent), id) == 1));
        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<HideWhenRemovalOrdered>().HandleAsync(new RemovalOrderReceivedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, id, 1), Cancellation);
        }

        Assert.Equal(author, database.Neighbours.Hidden[id].AuthorAccountId);
        Assert.NotNull((await FindAsync(id))!.HiddenAt);
        Assert.True(await ModerationTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(RemovalOrderExecutedEvent), id) == 1));
        Assert.True(await ModerationTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(ContentHiddenEvent), id) == 1));
    }

    [Fact]
    public async Task Without_the_confirmation_the_delivery_fails_and_the_retry_hides_it()
    {
        var (album, _, _) = database.Neighbours.NewAlbum();
        var id = Guid.CreateVersion7();
        await EnterAsync(id, nameof(OrderKind.RemoveContent), "Album", album);
        await ReportFlowTests.FireAsync(database);
        var received = new RemovalOrderReceivedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, id, 1);

        database.Neighbours.AccessDown = true;
        try
        {
            await using var scope = database.CreateScope();
            await Assert.ThrowsAsync<InvalidOperationException>(() => scope.ServiceProvider.GetRequiredService<HideWhenRemovalOrdered>().HandleAsync(received, Cancellation));
        }
        finally
        {
            database.Neighbours.AccessDown = false;
        }

        Assert.Null((await FindAsync(id))!.HiddenAt);
        await using var retry = database.CreateScope();
        await retry.ServiceProvider.GetRequiredService<HideWhenRemovalOrdered>().HandleAsync(received, Cancellation);
        Assert.NotNull((await FindAsync(id))!.HiddenAt);
    }

    [Fact]
    public async Task Data_is_secured_first_disclosed_only_then_with_an_entry_in_the_log_and_released_when_closed()
    {
        var account = Guid.CreateVersion7();
        var id = Guid.CreateVersion7();
        await EnterAsync(id, nameof(OrderKind.ProduceData), account: account);
        await ReportFlowTests.FireAsync(database);
        await StepAsync(database, id, OrderAction.Disclose);
        await ReportFlowTests.FireAsync(database);
        Assert.Null((await FindAsync(id))!.DisclosedAt);

        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<SecureWhenDataOrdered>().HandleAsync(new DataOrderReceivedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, id, 1), Cancellation);
        }

        Assert.True(await HeldAsync(account));
        Assert.True(await ModerationTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(AccountDataSecuredEvent), id) == 1));

        await StepAsync(database, id, OrderAction.Disclose);
        await ReportFlowTests.FireAsync(database);
        Assert.NotNull((await FindAsync(id))!.DisclosedAt);
        await using (var connection = await database.OpenAsAsync(ModerationTestDatabase.Role))
        {
            await using var command = new NpgsqlCommand("SELECT count(*) FROM moderation.access_log WHERE subject_id = @id AND kind = 'Disclosure' AND concerns_id = @account", connection);
            command.Parameters.AddWithValue("id", id);
            command.Parameters.AddWithValue("account", account);
            Assert.Equal(1L, await command.ExecuteScalarAsync(Cancellation));
        }

        await StepAsync(database, id, OrderAction.Close);
        await ReportFlowTests.FireAsync(database);
        Assert.False(await HeldAsync(account));
    }

    [Fact]
    public async Task An_order_to_preserve_is_never_disclosed_and_what_is_not_an_order_is_refused()
    {
        var account = Guid.CreateVersion7();
        var id = Guid.CreateVersion7();
        var wrong = Guid.CreateVersion7();
        await EnterAsync(id, nameof(OrderKind.PreserveData), account: account);
        await EnterAsync(wrong, nameof(OrderKind.RemoveContent));
        await ReportFlowTests.FireAsync(database);
        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<SecureWhenDataOrdered>().HandleAsync(new DataOrderReceivedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, id, 1), Cancellation);
            Assert.Equal(OrderErrors.WrongKind, (await scope.ServiceProvider.GetRequiredService<OrderHandler>().DiscloseAsync(new OrderId(id), "x", "test", Cancellation)).Error);
        }

        Assert.True(await HeldAsync(account));
        Assert.Null(await FindAsync(wrong));
        await using var connection = await database.OpenAsAsync(ModerationTestDatabase.Role);
        await using var command = new NpgsqlCommand("SELECT result FROM moderation.incoming_orders WHERE id = @id", connection);
        command.Parameters.AddWithValue("id", wrong);
        Assert.Equal("Moderation.InvalidOrder", await command.ExecuteScalarAsync(Cancellation));
    }
}

/// <summary>SD6 and P19: E115 delivered twice hides once.</summary>
[Collection(nameof(OperatorJobs))]
public sealed class RemovalOrderReceivedHandlerIdempotencyTests(ModerationTestDatabase database)
{
    [Fact]
    public async Task Twice_is_once()
    {
        var (memory, _, _) = database.Neighbours.NewMemory();
        var id = Guid.CreateVersion7();
        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<OrderHandler>().HandleAsync(
                new ReceiveOrder(new OrderId(id), nameof(OrderKind.RemoveContent), "Sąd", "Sygn. 1", "Memory", memory, null, false, "test"), TestContext.Current.CancellationToken);
        }

        var received = new RemovalOrderReceivedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, id, 1);
        for (var delivery = 0; delivery < 2; delivery++)
        {
            await using var scope = database.CreateScope();
            await scope.ServiceProvider.GetRequiredService<HideWhenRemovalOrdered>().HandleAsync(received, TestContext.Current.CancellationToken);
        }

        Assert.True(await ModerationTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(RemovalOrderExecutedEvent), id) == 1));
    }
}

/// <summary>SD6 and P20: E116 delivered twice secures once.</summary>
[Collection(nameof(OperatorJobs))]
public sealed class DataOrderReceivedHandlerIdempotencyTests(ModerationTestDatabase database)
{
    [Fact]
    public async Task Twice_is_once()
    {
        var id = Guid.CreateVersion7();
        await using (var scope = database.CreateScope())
        {
            await scope.ServiceProvider.GetRequiredService<OrderHandler>().HandleAsync(
                new ReceiveOrder(new OrderId(id), nameof(OrderKind.PreserveData), "Sąd", "Sygn. 2", null, null, Guid.CreateVersion7(), true, "test"), TestContext.Current.CancellationToken);
        }

        var received = new DataOrderReceivedEvent(Guid.CreateVersion7(), DateTimeOffset.UtcNow, id, 1);
        for (var delivery = 0; delivery < 2; delivery++)
        {
            await using var scope = database.CreateScope();
            await scope.ServiceProvider.GetRequiredService<SecureWhenDataOrdered>().HandleAsync(received, TestContext.Current.CancellationToken);
        }

        Assert.True(await ModerationTestDatabase.EventuallyAsync(async () => await database.QueuedEventsAsync(nameof(AccountDataSecuredEvent), id) == 1));
    }
}
