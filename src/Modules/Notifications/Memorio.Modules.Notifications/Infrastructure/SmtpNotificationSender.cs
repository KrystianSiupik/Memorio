using MailKit.Net.Smtp;
using MailKit.Security;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Notifications.Application;
using Memorio.Modules.Notifications.Application.Ports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Memorio.Modules.Notifications.Infrastructure;

/// <summary>
/// Sends through any SMTP provider (D-265). A provider that does not answer fails the send, so the event is
/// retried and <see cref="SendOnce"/> finishes it. An address the provider refuses for good is not retried:
/// another attempt will not help, and it would end in the dead letters. Logs never carry the address (D-162).
/// </summary>
internal sealed partial class SmtpNotificationSender(
    IOptions<NotificationsOptions> options,
    IAlbumNames albums,
    ILogger<SmtpNotificationSender> logger) : INotificationSender
{
    public async Task SendAsync(string email, NotificationKind kind, Guid albumId, string? invitedBy, CancellationToken cancellationToken)
    {
        // This sender is registered only with a host, and the options require a sender address with it.
        var smtp = options.Value.Smtp;
        var names = await albums.NamesOfAsync([albumId], cancellationToken);
        var (subject, body) = NotificationTexts.Of(kind, names.GetValueOrDefault(albumId), smtp.AppUrl, invitedBy);

        using var message = new MimeMessage();
        message.From.Add(new MailboxAddress(smtp.FromName, smtp.From!));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(smtp.Host!, smtp.Port, SecurityOf(smtp.Security), cancellationToken);
        if (!string.IsNullOrEmpty(smtp.Username))
        {
            await client.AuthenticateAsync(smtp.Username, smtp.Password ?? "", cancellationToken);
        }

        try
        {
            await client.SendAsync(message, cancellationToken);
        }
        catch (SmtpCommandException exception) when (exception.ErrorCode == SmtpErrorCode.RecipientNotAccepted && (int)exception.StatusCode >= 500)
        {
            LogRefused(logger, kind, albumId);
        }

        await client.DisconnectAsync(quit: true, cancellationToken);
    }

    private static SecureSocketOptions SecurityOf(string security) => security switch
    {
        "SslOnConnect" => SecureSocketOptions.SslOnConnect,
        "None" => SecureSocketOptions.None,
        _ => SecureSocketOptions.StartTls,
    };

    [LoggerMessage(Level = LogLevel.Warning, Message = "Message {Kind} about album {AlbumId} refused for good by the provider.")]
    private static partial void LogRefused(ILogger logger, NotificationKind kind, Guid albumId);
}
