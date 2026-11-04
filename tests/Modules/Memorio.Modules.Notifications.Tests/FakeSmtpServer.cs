using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Memorio.Modules.Notifications.Tests;

/// <summary>The little of SMTP a client needs, on a local port; keeps what it received.</summary>
public sealed class FakeSmtpServer : IAsyncDisposable
{
    private readonly TcpListener _listener = new(IPAddress.Loopback, 0);
    private readonly CancellationTokenSource _stop = new();
    private readonly Task _loop;

    public FakeSmtpServer(bool refusesRecipients = false)
    {
        RefusesRecipients = refusesRecipients;
        _listener.Start();
        _loop = Task.Run(AcceptAsync);
    }

    public bool RefusesRecipients { get; }

    public int Port => ((IPEndPoint)_listener.LocalEndpoint).Port;

    public List<string> Received { get; } = [];

    public async ValueTask DisposeAsync()
    {
        await _stop.CancelAsync();
        _listener.Stop();
        await _loop;
        _stop.Dispose();
    }

    private async Task AcceptAsync()
    {
        try
        {
            while (true)
            {
                using var client = await _listener.AcceptTcpClientAsync(_stop.Token);
                await ServeAsync(client);
            }
        }
        catch (Exception exception) when (exception is OperationCanceledException or SocketException or IOException or InvalidOperationException)
        {
            // Stopped.
        }
    }

    private async Task ServeAsync(TcpClient client)
    {
        await using var stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.Latin1);
        await using var writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true, NewLine = "\r\n" };
        await writer.WriteLineAsync("220 fake");
        while (await reader.ReadLineAsync(_stop.Token) is { } line)
        {
            var command = line.Split(' ', ':')[0].ToUpperInvariant();
            switch (command)
            {
                case "RCPT" when RefusesRecipients:
                    await writer.WriteLineAsync("550 no such mailbox");
                    break;
                case "DATA":
                    await writer.WriteLineAsync("354 go");
                    var data = new StringBuilder();
                    while (await reader.ReadLineAsync(_stop.Token) is { } content && content != ".")
                    {
                        data.Append(content).Append("\r\n");
                    }

                    Received.Add(data.ToString());
                    await writer.WriteLineAsync("250 taken");
                    break;
                case "QUIT":
                    await writer.WriteLineAsync("221 bye");
                    return;
                default:
                    await writer.WriteLineAsync("250 ok");
                    break;
            }
        }
    }
}
