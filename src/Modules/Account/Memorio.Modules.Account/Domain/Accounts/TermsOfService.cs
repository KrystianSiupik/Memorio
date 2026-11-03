namespace Memorio.Modules.Account.Domain.Accounts;

/// <summary>The terms a person accepts before adding their first content (C2).</summary>
internal static class TermsOfService
{
    /// <summary>Changing it asks everybody again: a person's acceptance stands for the version they accepted.</summary>
    public const string CurrentVersion = "2026-11";
}
