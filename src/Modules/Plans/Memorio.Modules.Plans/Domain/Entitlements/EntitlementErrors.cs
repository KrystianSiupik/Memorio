using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Plans.Domain.Entitlements;

internal static class EntitlementErrors
{
    public static readonly Error SourceRequired = Error.Validation(
        "Plans.EntitlementSourceRequired",
        "A ledger line needs its source and the source's reference of the notification.");

    public static readonly Error ResourceRequired = Error.Validation(
        "Plans.EntitlementResourceRequired",
        "A ledger line needs the account or album it entitles.");

    public static readonly Error ReasonRequired = Error.Validation(
        "Plans.EntitlementReasonRequired",
        "A manual ledger line needs the reason it was granted or ended.");
}
