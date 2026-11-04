using System.Text;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Notifications.Application;
using Memorio.Modules.Notifications.Application.Ports;
using Memorio.Modules.Notifications.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Memorio.Modules.Notifications.Tests;

/// <summary>D-265: a real SMTP conversation with a local server, no provider involved.</summary>
public sealed class SmtpNotificationSenderTests
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    [Fact]
    public async Task A_message_goes_out_in_polish_with_the_album_and_the_address_of_the_app()
    {
        await using var server = new FakeSmtpServer();
        var album = Guid.CreateVersion7();

        await Sender(server.Port, new Names(album, "Rodzina")).SendAsync("ania@example.invalid", NotificationKind.Invitation, album, Cancellation);

        using var message = MimeMessage.Load(new MemoryStream(Encoding.Latin1.GetBytes(Assert.Single(server.Received))), Cancellation);
        Assert.Equal("Zaproszenie do albumu w GEO", message.Subject);
        Assert.Equal("ania@example.invalid", message.To.Mailboxes.Single().Address);
        Assert.Equal("powiadomienia@example.invalid", message.From.Mailboxes.Single().Address);
        Assert.Contains("album „Rodzina”", message.TextBody);
        Assert.Contains("https://app.example.invalid", message.TextBody);
    }

    [Fact]
    public async Task An_address_refused_for_good_is_not_retried()
    {
        await using var server = new FakeSmtpServer(refusesRecipients: true);
        var album = Guid.CreateVersion7();

        await Sender(server.Port, new Names(album, "Rodzina")).SendAsync("nikt@example.invalid", NotificationKind.Tagged, album, Cancellation);

        Assert.Empty(server.Received);
    }

    [Fact]
    public async Task A_provider_that_does_not_answer_fails_so_the_event_is_retried()
    {
        int port;
        await using (var server = new FakeSmtpServer())
        {
            port = server.Port;
        }

        await Assert.ThrowsAnyAsync<Exception>(() => Sender(port, new Names(Guid.Empty, "")).SendAsync("ania@example.invalid", NotificationKind.Tagged, Guid.CreateVersion7(), Cancellation));
    }

    [Theory]
    [InlineData(NotificationKind.Invitation)]
    [InlineData(NotificationKind.InvitationReminder)]
    [InlineData(NotificationKind.PoolFull)]
    [InlineData(NotificationKind.ChooseKeepOrTake)]
    [InlineData(NotificationKind.Tagged)]
    internal void Every_kind_has_a_text_with_the_album_on_one_line(NotificationKind kind)
    {
        var (subject, body) = NotificationTexts.Of(kind, "Ro\r\ndzina", null);

        Assert.NotEmpty(subject);
        Assert.Contains("album „Ro dzina”", body);
    }

    [Fact]
    public void A_deleted_album_still_has_a_text() =>
        Assert.Contains("album", NotificationTexts.Of(NotificationKind.Tagged, null, null).Body);

    [Theory]
    [InlineData("smtp.example.invalid", typeof(SmtpNotificationSender))]
    [InlineData(null, typeof(LoggingNotificationSender))]
    public void A_host_turns_real_sending_on(string? host, Type expected)
    {
        var services = new ServiceCollection();
        new NotificationsModule().AddServices(services, new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Notifications:ConnectionString"] = "Host=unused",
            ["Notifications:Smtp:Host"] = host,
            ["Notifications:Smtp:From"] = "powiadomienia@example.invalid",
            ["Database:MigrateOnStartup"] = "false",
        }).Build());

        Assert.Equal(expected, services.Single(service => service.ServiceType == typeof(INotificationSender)).ImplementationType);
    }

    private static SmtpNotificationSender Sender(int port, IAlbumNames names) => new(
        Options.Create(new NotificationsOptions
        {
            Smtp = new NotificationsSmtpOptions
            {
                Host = "127.0.0.1",
                Port = port,
                Security = "None",
                From = "powiadomienia@example.invalid",
                AppUrl = "https://app.example.invalid",
            },
        }),
        names,
        NullLogger<SmtpNotificationSender>.Instance);

    private sealed class Names(Guid album, string name) : IAlbumNames
    {
        public Task<IReadOnlyDictionary<Guid, string>> NamesOfAsync(IReadOnlyCollection<Guid> albumIds, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyDictionary<Guid, string>>(new Dictionary<Guid, string> { [album] = name });
    }
}
