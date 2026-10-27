using Memorio.Modules.Plans.Domain.Entitlements;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Plans.Application.Commands.RecordEntitlement;

/// <summary>A payment source's notification, translated by its adapter into the ledger's terms (D-078).</summary>
internal sealed record RecordEntitlement(
    string Source,
    string SourceReference,
    EntitledResource Resource,
    DateTimeOffset ValidUntil,
    DateTimeOffset OccurredAt,
    string? Note = null);

/// <summary>
/// The only way into the ledger for a payment source adapter. Recording the same notification again (a
/// retried webhook, a redelivered message) changes nothing: true when the line is new, false when it was there.
/// </summary>
internal sealed class RecordEntitlementHandler(IEntitlementLedger ledger, TimeProvider clock)
{
    public async Task<Result<bool>> HandleAsync(RecordEntitlement command, CancellationToken cancellationToken)
    {
        var entry = EntitlementEntry.Create(
            command.Source,
            command.SourceReference,
            command.Resource,
            command.ValidUntil,
            command.OccurredAt,
            clock.GetUtcNow(),
            command.Note);
        if (entry.IsFailure)
        {
            return entry.Error;
        }

        return await ledger.AppendAsync(entry.Value, cancellationToken);
    }
}
