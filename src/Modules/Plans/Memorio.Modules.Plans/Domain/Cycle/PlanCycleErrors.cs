using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Plans.Domain.Cycle;

internal static class PlanCycleErrors
{
    public static readonly Error NoAccess = Error.Forbidden("Plans.NoAccess", "Only the owner decides about this plan.");

    public static readonly Error ChoiceClosed = Error.Conflict("Plans.ChoiceClosed", "The window to choose what stays is not open.");

    public static readonly Error NotPaid = Error.Conflict("Plans.NotPaid", "The plan is not paid.");

    public static readonly Error AlreadyDecided = Error.Conflict("Plans.AlreadyDecided", "Something else was decided about this plan at the same time.");
}
