namespace Memorio.Modules.Notifications.Infrastructure;

/// <summary>Configuration section "Notifications".</summary>
internal sealed class NotificationsOptions
{
    public const string Section = "Notifications";

    /// <summary>PostgreSQL connection of the role memorio_notifications (schema notifications, ADR 0004). Required.</summary>
    public string? ConnectionString { get; set; }
}
