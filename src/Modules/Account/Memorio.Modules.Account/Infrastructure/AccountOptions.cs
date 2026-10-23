namespace Memorio.Modules.Account.Infrastructure;

/// <summary>Configuration section "Account".</summary>
internal sealed class AccountOptions
{
    public const string Section = "Account";

    /// <summary>PostgreSQL connection of the role memorio_account (schema account, ADR 0004). Required.</summary>
    public string? ConnectionString { get; set; }

    public ProviderOptions Google { get; set; } = new();

    public ProviderOptions Apple { get; set; } = new();

    internal sealed class ProviderOptions
    {
        /// <summary>Our app ids at the provider (ID token audiences: iOS, Android, web); none = sign-in off.</summary>
        public string[] ClientIds { get; set; } = [];
    }
}
