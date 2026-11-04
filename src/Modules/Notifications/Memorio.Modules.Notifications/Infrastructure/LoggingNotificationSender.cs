using Memorio.Modules.Notifications.Application.Ports;
using Microsoft.Extensions.Logging;

namespace Memorio.Modules.Notifications.Infrastructure;

/// <summary>
/// **Delivers nothing**: for a host without Notifications__Smtp__Host, e.g. local work (D-265). Writes that a message
/// would go, without the address (D-162), so the rest of the flow can run and be seen.
/// </summary>
internal sealed partial class LoggingNotificationSender(ILogger<LoggingNotificationSender> logger) : INotificationSender
{
    public Task SendAsync(string email, NotificationKind kind, Guid albumId, CancellationToken cancellationToken)
    {
        LogNotSent(logger, kind, albumId);
        return Task.CompletedTask;
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Message {Kind} about album {AlbumId} not delivered: no provider is set up.")]
    private static partial void LogNotSent(ILogger logger, NotificationKind kind, Guid albumId);
}
