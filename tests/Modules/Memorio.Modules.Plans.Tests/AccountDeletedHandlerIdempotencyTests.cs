using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Plans.Application.Commands.Cycle;
using Memorio.Modules.Plans.Application.Commands.RecordEntitlement;
using Memorio.Modules.Plans.Application.Policies;
using Memorio.Modules.Plans.Domain.Cycle;
using Memorio.Modules.Plans.Domain.Entitlements;
using Microsoft.Extensions.DependencyInjection;

namespace Memorio.Modules.Plans.Tests;

/// <summary>SD6: E7 delivered twice, on a real database.</summary>
public sealed class AccountDeletedHandlerIdempotencyTests(PlansDatabase database)
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task A_deleted_account_has_a_closed_cycle_that_no_line_opens_again()
    {
        var account = PlansDatabase.NewAccount();
        var now = DateTimeOffset.UtcNow;
        await using var scope = database.CreateScope();
        var services = scope.ServiceProvider;
        await services.GetRequiredService<RecordEntitlementHandler>().HandleAsync(new RecordEntitlement("manual", "first", account, now.AddMonths(1), now, "test"), Cancellation);
        await services.GetRequiredService<AdvanceCycleHandler>().HandleAsync(account, now, Cancellation);

        await using (var delivery = database.CreateScope())
        {
            var deleted = new AccountDeleted(Guid.CreateVersion7(), now, account.Id, 2);
            var policy = delivery.ServiceProvider.GetRequiredService<CloseCycleWhenAccountDeleted>();
            await policy.HandleAsync(deleted, Cancellation);
            await policy.HandleAsync(deleted, Cancellation);
        }

        await using var check = database.CreateScope();
        await check.ServiceProvider.GetRequiredService<AdvanceCycleHandler>().HandleAsync(account, now.AddDays(40), Cancellation);
        Assert.Equal(PlanMode.Closed, (await check.ServiceProvider.GetRequiredService<IPlanCycles>().FindAsync(account, Cancellation))!.Mode);
    }

    [Fact]
    public async Task An_account_that_never_paid_is_closed_too()
    {
        var account = PlansDatabase.NewAccount();
        await using var scope = database.CreateScope();

        await scope.ServiceProvider.GetRequiredService<CloseCycleWhenAccountDeleted>().HandleAsync(new AccountDeleted(Guid.CreateVersion7(), DateTimeOffset.UtcNow, account.Id, 2), Cancellation);

        Assert.Equal(PlanMode.Closed, (await scope.ServiceProvider.GetRequiredService<IPlanCycles>().FindAsync(account, Cancellation))!.Mode);
    }
}
