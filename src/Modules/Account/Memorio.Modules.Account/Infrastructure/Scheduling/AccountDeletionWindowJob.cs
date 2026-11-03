using System.Globalization;
using Memorio.Modules.Account.Application.Policies;
using Memorio.Modules.Account.Domain.Accounts;
using Memorio.SharedKernel.Scheduling;

namespace Memorio.Modules.Account.Infrastructure.Scheduling;

/// <summary>The deadline of a window firing is E6; decides with the time it was due, not with "now" (ADR 0012).</summary>
internal sealed class AccountDeletionWindowJob(DeleteAccountWhenWindowPassed policy) : IScheduledJob
{
    public Task RunAsync(ScheduledJobRun run, CancellationToken cancellationToken) =>
        policy.HandleAsync(
            new DeletionWindowPassed(
                new AccountId(Guid.Parse(run.Key!)),
                int.Parse(run.Data[AccountDeletionWindowDeadline.SequenceKey], CultureInfo.InvariantCulture),
                run.ScheduledFor),
            cancellationToken);
}
