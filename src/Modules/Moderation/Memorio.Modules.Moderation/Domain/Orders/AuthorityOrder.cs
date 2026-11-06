using Memorio.Modules.Moderation.Domain.Reports;
using Memorio.SharedKernel.Domain;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Moderation.Domain.Orders;

internal readonly record struct OrderId(Guid Value);

/// <summary>Remove: content within an hour (L11). Preserve: keep an account's data without disclosing it. Produce: disclose it (L18).</summary>
internal enum OrderKind
{
    RemoveContent,
    PreserveData,
    ProduceData,
}

/// <summary>
/// An order of an authority the service checked and entered (Z6, D-312). Its id is the hold on the content in Access
/// for a removal, and the legal hold on the account for data: nothing of it goes for good until the order is closed.
/// Every disclosure is an entry in the log (S2:C8). <see cref="Sequence"/> is the version.
/// </summary>
internal sealed class AuthorityOrder : AggregateRoot<OrderId>
{
    public const int MaximumTextLength = 1_000;

    private AuthorityOrder(OrderId id)
        : base(id)
    {
    }

    public OrderKind Kind { get; private set; }

    public string Authority { get; private set; } = "";

    /// <summary>The case number and the legal basis, as the order gives them.</summary>
    public string Reference { get; private set; } = "";

    public ReportTarget? ResourceKind { get; private set; }

    public Guid? ResourceId { get; private set; }

    public Guid? AccountId { get; private set; }

    /// <summary>The authority asked that the author is not told (L11, L18).</summary>
    public bool Confidential { get; private set; }

    public string ReceivedBy { get; private set; } = "";

    public DateTimeOffset ReceivedAt { get; private set; }

    public DateTimeOffset? HiddenAt { get; private set; }

    public DateTimeOffset? SecuredAt { get; private set; }

    public DateTimeOffset? DisclosedAt { get; private set; }

    public DateTimeOffset? ClosedAt { get; private set; }

    public int Sequence { get; private set; } = 1;

    /// <summary>E115 or E116: the order is in; P19 or P20 acts on it at once.</summary>
    public static Result<AuthorityOrder> Receive(
        OrderId id, string? kind, string? authority, string? reference, string? resourceKind, Guid? resourceId, Guid? accountId, bool confidential, string? receivedBy, DateTimeOffset now)
    {
        var who = authority?.Trim();
        var basis = reference?.Trim();
        if (id.Value == Guid.Empty || kind is null || !Enum.GetNames<OrderKind>().Contains(kind) || !Enum.TryParse<OrderKind>(kind, out var parsed)
            || string.IsNullOrEmpty(who) || who.Length > MaximumTextLength || string.IsNullOrEmpty(basis) || basis.Length > MaximumTextLength
            || string.IsNullOrWhiteSpace(receivedBy))
        {
            return OrderErrors.InvalidOrder;
        }

        var order = new AuthorityOrder(id) { Kind = parsed, Authority = who, Reference = basis, Confidential = confidential, ReceivedBy = receivedBy.Trim(), ReceivedAt = now };
        if (parsed == OrderKind.RemoveContent)
        {
            if (resourceKind is not (nameof(ReportTarget.Memory) or nameof(ReportTarget.Album)) || resourceId is null || resourceId == Guid.Empty)
            {
                return OrderErrors.InvalidOrder;
            }

            order.ResourceKind = Enum.Parse<ReportTarget>(resourceKind);
            order.ResourceId = resourceId;
            order.Raise(new RemovalOrderReceived(Guid.CreateVersion7(), now, id, order.Sequence));
            return order;
        }

        if (accountId is null || accountId == Guid.Empty)
        {
            return OrderErrors.InvalidOrder;
        }

        order.AccountId = accountId;
        order.Raise(new DataOrderReceived(Guid.CreateVersion7(), now, id, order.Sequence));
        return order;
    }

    /// <summary>C113: hidden from everyone but its author, confirmed by Access. Again changes nothing.</summary>
    public Result MarkHidden(Guid? authorAccountId, DateTimeOffset now)
    {
        if (Kind != OrderKind.RemoveContent)
        {
            return OrderErrors.WrongKind;
        }

        if (HiddenAt is not null)
        {
            return Result.Success();
        }

        HiddenAt = now;
        Sequence++;
        Raise(new ContentHiddenByOrder(Guid.CreateVersion7(), now, Id, ResourceKind!.Value, ResourceId!.Value, authorAccountId, Confidential, Sequence));
        return Result.Success();
    }

    /// <summary>C114, E117: from now nothing of the account goes for good, and nothing is disclosed by it. Again changes nothing.</summary>
    public Result Secure(DateTimeOffset now)
    {
        if (Kind == OrderKind.RemoveContent)
        {
            return OrderErrors.WrongKind;
        }

        if (SecuredAt is not null)
        {
            return Result.Success();
        }

        SecuredAt = now;
        Sequence++;
        Raise(new AccountDataSecured(Guid.CreateVersion7(), now, Id, AccountId!.Value, Sequence));
        return Result.Success();
    }

    /// <summary>C115, E118: only for an order to produce, after the data is secured (L18: preservation first), while open.</summary>
    public Result<AccessLogEntry> Disclose(string? what, string? by, DateTimeOffset now)
    {
        if (Kind != OrderKind.ProduceData)
        {
            return OrderErrors.WrongKind;
        }

        if (SecuredAt is null || ClosedAt is not null)
        {
            return OrderErrors.NotSecured;
        }

        var entry = AccessLogEntry.Disclosure(Id, AccountId!.Value, what, by, Reference, now);
        if (entry.IsFailure)
        {
            return entry;
        }

        DisclosedAt ??= now;
        Sequence++;
        return entry;
    }

    /// <summary>The authority no longer needs the data: the legal hold ends, and what waited to go, goes. A removal is never closed.</summary>
    public Result Close(DateTimeOffset now)
    {
        if (Kind == OrderKind.RemoveContent)
        {
            return OrderErrors.WrongKind;
        }

        if (ClosedAt is not null)
        {
            return Result.Success();
        }

        ClosedAt = now;
        Sequence++;
        return Result.Success();
    }
}

/// <summary>
/// S2:C8: who looked at content or disclosed data, when, what and on which basis. Only ever added: no code changes or
/// removes an entry, and the module has no other way to it.
/// </summary>
internal sealed class AccessLogEntry
{
    public const string View = "View";

    public const string DisclosureKind = "Disclosure";

    private AccessLogEntry()
    {
    }

    public Guid Id { get; private set; }

    public DateTimeOffset At { get; private set; }

    public string By { get; private set; } = "";

    public string Kind { get; private set; } = "";

    /// <summary>The report or the order it was done for.</summary>
    public Guid SubjectId { get; private set; }

    /// <summary>The memory, album or account it concerned.</summary>
    public Guid? ConcernsId { get; private set; }

    public string What { get; private set; } = "";

    public string Basis { get; private set; } = "";

    public static Result<AccessLogEntry> Disclosure(OrderId order, Guid accountId, string? what, string? by, string basis, DateTimeOffset now)
    {
        var text = what?.Trim();
        return string.IsNullOrEmpty(text) || text.Length > AuthorityOrder.MaximumTextLength || string.IsNullOrWhiteSpace(by)
            ? OrderErrors.InvalidOrder
            : new AccessLogEntry { Id = Guid.CreateVersion7(), At = now, By = by.Trim(), Kind = DisclosureKind, SubjectId = order.Value, ConcernsId = accountId, What = text, Basis = basis };
    }
}

/// <summary>E115.</summary>
internal sealed record RemovalOrderReceived(Guid EventId, DateTimeOffset OccurredAt, OrderId OrderId, int Sequence) : IDomainEvent;

/// <summary>E116.</summary>
internal sealed record DataOrderReceived(Guid EventId, DateTimeOffset OccurredAt, OrderId OrderId, int Sequence) : IDomainEvent;

internal sealed record ContentHiddenByOrder(
    Guid EventId, DateTimeOffset OccurredAt, OrderId OrderId, ReportTarget Target, Guid ResourceId, Guid? AuthorAccountId, bool Confidential, int Sequence) : IDomainEvent;

/// <summary>E117.</summary>
internal sealed record AccountDataSecured(Guid EventId, DateTimeOffset OccurredAt, OrderId OrderId, Guid AccountId, int Sequence) : IDomainEvent;

internal static class OrderErrors
{
    public static readonly Error InvalidOrder = Error.Validation(
        "Moderation.InvalidOrder",
        "An order needs its kind, the authority, the reference and who entered it; a removal a memory or an album, the others an account.");

    public static readonly Error WrongKind = Error.Validation("Moderation.WrongOrderKind", "This order is of another kind.");

    public static readonly Error NotSecured = Error.Conflict("Moderation.NotSecured", "The data is disclosed only after it is secured, while the order is open.");

    public static readonly Error OrderNotFound = Error.NotFound("Moderation.OrderNotFound", "There is no such order.");
}

/// <summary>An order as the service entered it through deploy/moderation, applied by the worker within a minute (D-312).</summary>
internal sealed class IncomingOrder
{
    private IncomingOrder()
    {
    }

    public Guid Id { get; private set; }

    public string Kind { get; private set; } = "";

    public string Authority { get; private set; } = "";

    public string Reference { get; private set; } = "";

    public string? ResourceKind { get; private set; }

    public Guid? ResourceId { get; private set; }

    public Guid? AccountId { get; private set; }

    public bool Confidential { get; private set; }

    public string ReceivedBy { get; private set; } = "";

    public DateTimeOffset ReceivedAt { get; private set; }

    public DateTimeOffset? AppliedAt { get; private set; }

    public string? Result { get; private set; }

    public void MarkApplied(string result, DateTimeOffset now)
    {
        Result = result;
        AppliedAt = now;
    }
}

/// <summary>A step on an order the service decided: Disclose (with what was handed over) or Close.</summary>
internal sealed class OrderAction
{
    public const string Disclose = "Disclose";

    public const string Close = "Close";

    private OrderAction()
    {
    }

    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }

    public string Kind { get; private set; } = "";

    public string Details { get; private set; } = "";

    public string DecidedBy { get; private set; } = "";

    public DateTimeOffset DecidedAt { get; private set; }

    public DateTimeOffset? AppliedAt { get; private set; }

    public string? Result { get; private set; }

    public void MarkApplied(string result, DateTimeOffset now)
    {
        Result = result;
        AppliedAt = now;
    }
}

internal interface IOrders
{
    Task<AuthorityOrder?> FindAsync(OrderId id, CancellationToken cancellationToken);

    void Add(AuthorityOrder order);

    void Log(AccessLogEntry entry);

    Task<IReadOnlyList<Guid>> FindIncomingAsync(CancellationToken cancellationToken);

    Task<IncomingOrder?> FindIncomingAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Guid>> FindPendingActionsAsync(CancellationToken cancellationToken);

    Task<OrderAction?> FindActionAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Stores the changes with their events, in one transaction. False when another decision got there first.</summary>
    Task<bool> SaveAsync(CancellationToken cancellationToken);
}
