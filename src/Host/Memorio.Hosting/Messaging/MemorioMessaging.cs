using JasperFx.CodeGeneration.Model;
using JasperFx.Resources;
using Memorio.Hosting.Configuration;
using Memorio.Hosting.Health;
using Memorio.SharedKernel.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Postgresql;

namespace Memorio.Hosting.Messaging;

/// <summary>
/// Outbox, inbox and queue in PostgreSQL with Wolverine, no broker (ADR 0006, D-030). Integration
/// events stored in a module's transaction go from the outbox to the PostgreSQL queue
/// <see cref="Queue"/>; only the worker listens to it, through the durable inbox (duplicates of a
/// message id are dropped), and gives every module handler its own durable local queue, so one
/// failing module neither blocks nor repeats the others. A message that still fails after the
/// retries goes to the dead letters, which the backlog metrics count (SD6, QS12).
/// </summary>
public static class MemorioMessaging
{
    public const string EnvelopeSchema = "wolverine";
    public const string QueueSchema = "wolverine_queues";
    public const string Queue = "integration_events";
    public const string HealthCheckName = "messaging-database";

    /// <summary>Waits between attempts of a failing handler; after the last one the message is dead-lettered.</summary>
    public static readonly TimeSpan[] RetryDelays = [TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(1)];

    /// <param name="handlerAssemblies">Where the worker finds handlers: public static *Handler classes (module assemblies).</param>
    public static IServiceCollection AddMemorioMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        MessagingHost host,
        IEnumerable<System.Reflection.Assembly> handlerAssemblies)
    {
        services.AddOptions<MessagingOptions>()
            .Bind(configuration.GetSection(MessagingOptions.Section))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.ConnectionString),
                $"{ConfigurationErrors.Variable($"{MessagingOptions.Section}:{nameof(MessagingOptions.ConnectionString)}")} is required.")
            .ValidateOnStart();

        // A missing value fails the validation above before Wolverine connects; the placeholder only
        // lets the host be built so that validation can report it.
        var connectionString = configuration.GetSection(MessagingOptions.Section).Get<MessagingOptions>()?.ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=unconfigured";
        }

        // No Kerberos: without this Npgsql probes GSS encryption and logs a missing libgssapi in containers.
        connectionString = new NpgsqlConnectionStringBuilder(connectionString) { GssEncryptionMode = GssEncryptionMode.Disable }
            .ConnectionString;

        services.AddSingleton(_ => new MessagingDatabase(NpgsqlDataSource.Create(connectionString)));
        services.AddHealthChecks().AddCheck<MessagingDatabaseHealthCheck>(HealthCheckName, tags: [HealthEndpoints.ReadyTag]);
        services.AddResourceSetupOnStartup();

        if (host == MessagingHost.Worker)
        {
            services.TryAddSingleton(TimeProvider.System);
            services.AddSingleton<BacklogMonitor>();
            services.AddHostedService(provider => provider.GetRequiredService<BacklogMonitor>());
        }

        services.AddWolverine(options =>
        {
            options.ServiceName = host == MessagingHost.Api ? "memorio-api" : "memorio-worker";
            options.UsePostgresqlPersistenceAndTransport(connectionString, EnvelopeSchema, transportSchema: QueueSchema)
                .AutoProvision();
            options.Publish(rule => rule.MessagesImplementing<IIntegrationEvent>().ToPostgresqlQueue(Queue));

            if (host == MessagingHost.Api)
            {
                options.Discovery.DisableConventionalDiscovery();
                return;
            }

            options.ListenToPostgresqlQueue(Queue).UseDurableInbox();
            options.MultipleHandlerBehavior = MultipleHandlerBehavior.Separated;
            options.Policies.UseDurableLocalQueues();
            options.Policies.OnAnyException().RetryWithCooldown(RetryDelays).Then.MoveToErrorQueue();

            // Handlers are thin public adapters; what they call is internal to their module, so they
            // resolve it from the message's service scope (D-135).
            options.ServiceLocationPolicy = ServiceLocationPolicy.AlwaysAllowed;
            foreach (var assembly in handlerAssemblies)
            {
                options.Discovery.IncludeAssembly(assembly);
            }
        });

        return services;
    }
}
