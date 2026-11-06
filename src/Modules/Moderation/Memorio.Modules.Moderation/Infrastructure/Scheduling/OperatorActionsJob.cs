using Memorio.Modules.Moderation.Application.Commands.Orders;
using Memorio.Modules.Moderation.Application.Commands.Reports;
using Memorio.Modules.Moderation.Domain.Orders;
using Memorio.Modules.Moderation.Domain.Reports;
using Memorio.SharedKernel.Results;
using Memorio.SharedKernel.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Memorio.Modules.Moderation.Infrastructure.Scheduling;

/// <summary>
/// What the service decided through deploy/moderation/moderation.sh, every minute (D-309): no public way in for the service,
/// and every decision goes through the same handler. Each has its own scope, and one failing does not stop others.
/// </summary>
internal sealed partial class OperatorActionsJob(IReports reports, IOrders orders, IServiceScopeFactory scopes, TimeProvider clock, ILogger<OperatorActionsJob> logger) : IScheduledJob
{
    public const string JobName = "Moderation.OperatorActions";

    public const string Cron = "0 * * * * ?";

    public async Task RunAsync(ScheduledJobRun run, CancellationToken cancellationToken)
    {
        var failed = 0;

        // Orders first: a removal must be done within an hour (L11).
        foreach (var id in await orders.FindIncomingAsync(cancellationToken))
        {
            failed += await TryAsync(id, async (services, own) =>
            {
                if (await own.FindIncomingAsync(id, cancellationToken) is not { AppliedAt: null } incoming)
                {
                    return;
                }

                var received = await services.GetRequiredService<OrderHandler>().HandleAsync(
                    new ReceiveOrder(new OrderId(incoming.Id), incoming.Kind, incoming.Authority, incoming.Reference, incoming.ResourceKind, incoming.ResourceId, incoming.AccountId, incoming.Confidential, incoming.ReceivedBy),
                    cancellationToken);
                incoming.MarkApplied(received.IsSuccess ? "Applied" : received.Error.Code, clock.GetUtcNow());
                if (!await own.SaveAsync(cancellationToken))
                {
                    throw new InvalidOperationException("Another run got there first.");
                }
            }, cancellationToken);
        }

        foreach (var id in await orders.FindPendingActionsAsync(cancellationToken))
        {
            failed += await TryAsync(id, async (services, own) =>
            {
                if (await own.FindActionAsync(id, cancellationToken) is not { AppliedAt: null } action)
                {
                    return;
                }

                var handler = services.GetRequiredService<OrderHandler>();
                var applied = action.Kind switch
                {
                    OrderAction.Disclose => await handler.DiscloseAsync(new OrderId(action.OrderId), action.Details, action.DecidedBy, cancellationToken),
                    OrderAction.Close => await handler.CloseAsync(new OrderId(action.OrderId), cancellationToken),
                    _ => OrderErrors.InvalidOrder,
                };
                action.MarkApplied(applied.IsSuccess ? "Applied" : applied.Error.Code, clock.GetUtcNow());
                if (!await own.SaveAsync(cancellationToken))
                {
                    throw new InvalidOperationException("Another run got there first.");
                }
            }, cancellationToken);
        }

        foreach (var id in await reports.FindPendingActionsAsync(cancellationToken))
        {
            try
            {
                await using var scope = scopes.CreateAsyncScope();
                var own = scope.ServiceProvider.GetRequiredService<IReports>();
                if (await own.FindActionAsync(id, cancellationToken) is not { AppliedAt: null } action)
                {
                    continue;
                }

                var applied = await ApplyAsync(scope.ServiceProvider.GetRequiredService<ReportDecisionHandler>(), action, cancellationToken);
                action.MarkApplied(applied.IsSuccess ? "Applied" : applied.Error.Code, clock.GetUtcNow());
                if (!await own.SaveAsync(cancellationToken))
                {
                    throw new InvalidOperationException("Another run got there first.");
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
            {
                LogFailed(logger, exception, id);
                failed++;
            }
        }

        if (failed > 0)
        {
            throw new InvalidOperationException($"{failed} actions failed.");
        }
    }

    private async Task<int> TryAsync(Guid id, Func<IServiceProvider, IOrders, Task> step, CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopes.CreateAsyncScope();
            await step(scope.ServiceProvider, scope.ServiceProvider.GetRequiredService<IOrders>());
            return 0;
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            LogFailed(logger, exception, id);
            return 1;
        }
    }

    private static Task<Result> ApplyAsync(ReportDecisionHandler handler, OperatorAction action, CancellationToken cancellationToken)
    {
        var report = new ReportId(action.ReportId);
        return (action.Kind, action.Outcome) switch
        {
            (OperatorAction.Decide, nameof(Verdict.Removed)) => handler.HandleAsync(new DecideReport(report, Verdict.Removed, action.Statement, action.DecidedBy), cancellationToken),
            (OperatorAction.Decide, nameof(Verdict.Kept)) => handler.HandleAsync(new DecideReport(report, Verdict.Kept, action.Statement, action.DecidedBy), cancellationToken),
            (OperatorAction.Decide, "Unfounded") => handler.HandleAsync(new DecideReport(report, Verdict.Kept, action.Statement, action.DecidedBy, Unfounded: true), cancellationToken),
            (OperatorAction.DecideAppeal, "Reversed") => handler.HandleAsync(new DecideAppeal(report, Reversed: true, action.Statement, action.DecidedBy), cancellationToken),
            (OperatorAction.DecideAppeal, "Upheld") => handler.HandleAsync(new DecideAppeal(report, Reversed: false, action.Statement, action.DecidedBy), cancellationToken),
            _ => Task.FromResult<Result>(ReportErrors.InvalidDecision),
        };
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Applying the service's action {ActionId} failed")]
    private static partial void LogFailed(ILogger logger, Exception exception, Guid actionId);
}
