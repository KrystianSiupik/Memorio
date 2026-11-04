namespace Memorio.Modules.Notifications.Infrastructure;

/// <summary>Configuration section "Notifications".</summary>
internal sealed class NotificationsOptions
{
    public const string Section = "Notifications";

    /// <summary>PostgreSQL connection of the role memorio_notifications (schema notifications, ADR 0004). Required.</summary>
    public string? ConnectionString { get; set; }

    /// <summary>Where messages go out. Without a host nothing is delivered, only logged (D-265).</summary>
    public NotificationsSmtpOptions Smtp { get; set; } = new();
}

/// <summary>Plain SMTP, so changing the provider is a change of these values (D-265).</summary>
internal sealed class NotificationsSmtpOptions
{
    public string? Host { get; set; }

    public int Port { get; set; } = 587;

    /// <summary>StartTls, SslOnConnect, or None for a test server only.</summary>
    public string Security { get; set; } = "StartTls";

    public string? Username { get; set; }

    public string? Password { get; set; }

    /// <summary>The sending address, on a domain the provider verified. Required with a host.</summary>
    public string? From { get; set; }

    public string FromName { get; set; } = "GEO";

    /// <summary>Where the app opens; shown in messages when set.</summary>
    public string? AppUrl { get; set; }
}
