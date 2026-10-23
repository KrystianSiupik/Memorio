using System.Diagnostics.Metrics;
using Memorio.Modules.Account.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Memorio.Modules.Account.Infrastructure.Metrics;

/// <summary>
/// Meter <see cref="MeterName"/>: <c>memorio.accounts.count</c>, the number of accounts, sampled every
/// <see cref="SampleInterval"/>. With the infrastructure bill it gives the cost per user (SD14); only a
/// count leaves the module, nothing about any person.
/// </summary>
internal sealed partial class AccountMetrics : BackgroundService
{
    public const string MeterName = "Memorio.Account";

    public static readonly TimeSpan SampleInterval = TimeSpan.FromMinutes(5);

    /// <summary>The first sample waits for the host to settle (and for the migrations of a fresh database).</summary>
    private static readonly TimeSpan FirstSampleDelay = TimeSpan.FromSeconds(30);

    private readonly IServiceScopeFactory _scopes;
    private readonly TimeProvider _time;
    private readonly ILogger<AccountMetrics> _logger;
    private long? _accounts;

    public AccountMetrics(IServiceScopeFactory scopes, TimeProvider time, IMeterFactory meterFactory, ILogger<AccountMetrics> logger)
    {
        _scopes = scopes;
        _time = time;
        _logger = logger;
        meterFactory.Create(MeterName).CreateObservableGauge(
            "memorio.accounts.count",
            () => _accounts is { } accounts ? [new Measurement<long>(accounts)] : Array.Empty<Measurement<long>>(),
            unit: "{account}",
            description: "Accounts that exist (SD14: cost per user).");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(FirstSampleDelay, _time, stoppingToken);
        using var timer = new PeriodicTimer(SampleInterval, _time);
        do
        {
            await SampleAsync(stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    internal async Task SampleAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = _scopes.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
            _accounts = await database.Users.LongCountAsync(cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            // The metric keeps its last value; readiness reports the database itself.
            LogSampleFailed(_logger, exception);
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Could not count the accounts for the metrics.")]
    private static partial void LogSampleFailed(ILogger logger, Exception exception);
}
