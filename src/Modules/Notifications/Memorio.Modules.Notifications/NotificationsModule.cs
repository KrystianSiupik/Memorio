using Memorio.Modularity;
using Memorio.Modules.Notifications.Api;
using Memorio.Modules.Notifications.Application;
using Memorio.Modules.Notifications.Application.Commands;
using Memorio.Modules.Notifications.Application.Policies;
using Memorio.Modules.Notifications.Application.Ports;
using Memorio.Modules.Notifications.Contracts;
using Memorio.Modules.Notifications.Domain;
using Memorio.Modules.Notifications.Infrastructure;
using Memorio.Modules.Notifications.Infrastructure.Persistence;
using Memorio.Modules.Notifications.Infrastructure.Scheduling;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wolverine.EntityFrameworkCore;

namespace Memorio.Modules.Notifications;

/// <summary>Module Notifications. Key question: Did the message arrive, and whom must we not write to?</summary>
public sealed class NotificationsModule : IModule
{
    public string Name => "Notifications";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<NotificationsOptions>()
            .Bind(configuration.GetSection(NotificationsOptions.Section))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "Notifications__ConnectionString is required.")
            .Validate(options => string.IsNullOrWhiteSpace(options.Smtp.Host) || !string.IsNullOrWhiteSpace(options.Smtp.From), "Notifications__Smtp__From is required with a host.")
            .ValidateOnStart();

        // A missing value fails the validation above at startup; the placeholder only lets the host be built.
        var connectionString = configuration.GetSection(NotificationsOptions.Section).Get<NotificationsOptions>()?.ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=unconfigured";
        }

        services.TryAddSingleton(TimeProvider.System);
        services.AddDbContextWithWolverineIntegration<NotificationsDbContext>(database => database.UseNotificationsDatabase(connectionString));
        services.AddScoped<INotificationLedger, NotificationLedger>();
        services.AddScoped<IInvitationBlocks, InvitationBlocks>();
        services.AddScoped<BlockInvitationsHandler>();
        services.AddScoped<SendOnce>();
        if (!string.IsNullOrWhiteSpace(configuration.GetSection(NotificationsOptions.Section).Get<NotificationsOptions>()?.Smtp.Host))
        {
            services.TryAddScoped<INotificationSender, SmtpNotificationSender>();
        }

        services.TryAddScoped<INotificationSender, LoggingNotificationSender>();
        services.AddScoped<SendInvitation>();
        services.AddScoped<SendInvitationReminder>();
        services.AddScoped<NotifyOwnerOfFullPool>();
        services.AddScoped<AskToChooseKeepOrTake>();
        services.AddScoped<NotifyTagged>();
        services.AddScoped<ITrimWarningReports, TrimWarningReports>();
        services.AddScoped<WarnBeforeTrimHandler>();
        services.AddScoped<WarnWhenDue>();
        services.AddScoped<RetryWhileHeld>();
        services.AddScheduledJob<TrimWarningRetryJob>(RetryWhileHeld.JobName);

        services.AddHealthChecks().AddCheck<NotificationsDatabaseHealthCheck>(NotificationsDatabaseHealthCheck.Name, tags: [HealthTags.Ready]);
        if (configuration.GetValue("Database:MigrateOnStartup", defaultValue: true))
        {
            services.AddHostedService<NotificationsDatabaseMigrator>();
        }
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints.MapNotificationsEndpoints();
}
