namespace Memorio.Modules.Plans.Domain.Entitlements;

/// <summary>What a plan is paid for: an account (personal or family plan) or an album (album plan, D-004).</summary>
internal enum EntitledResourceKind
{
    Account,
    Album,
}

/// <summary>An account or album id of the module that owns it; the ledger knows nothing else about it.</summary>
internal readonly record struct EntitledResource(EntitledResourceKind Kind, Guid Id);
