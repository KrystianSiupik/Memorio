using Memorio.Modularity;
using Memorio.Modules.Album.Api;
using Memorio.Modules.Album.Application.Commands.Contributions;
using Memorio.Modules.Album.Application.Commands.CreateAlbum;
using Memorio.Modules.Album.Application.Commands.Invitations;
using Memorio.Modules.Album.Application.Commands.Takeover;
using Memorio.Modules.Album.Application.Policies;
using Memorio.Modules.Album.Application.Queries;
using Memorio.Modules.Album.Contracts;
using Memorio.Modules.Album.Domain.Albums;
using Memorio.Modules.Album.Infrastructure.Scheduling;
using Memorio.SharedKernel.Scheduling;
using Memorio.Modules.Album.Infrastructure;
using Memorio.Modules.Album.Infrastructure.Persistence;
using Memorio.Modules.Album.Infrastructure.Queries;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wolverine.EntityFrameworkCore;

namespace Memorio.Modules.Album;

/// <summary>Module Album. Key question: Who builds this shared collection together and what did each of them bring?</summary>
public sealed class AlbumModule : IModule
{
    public string Name => "Album";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AlbumOptions>()
            .Bind(configuration.GetSection(AlbumOptions.Section))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "Album__ConnectionString is required.")
            .ValidateOnStart();

        // A missing value fails the validation above at startup; the placeholder only lets the host be built.
        var connectionString = configuration.GetSection(AlbumOptions.Section).Get<AlbumOptions>()?.ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=unconfigured";
        }

        services.TryAddSingleton(TimeProvider.System);
        services.AddDbContextWithWolverineIntegration<AlbumDbContext>(database => database.UseAlbumDatabase(connectionString));
        services.AddScoped<IAccountAlbumsReader, AccountAlbumsReader>();
        services.AddScoped<IAlbums, Albums>();
        services.AddScoped<IAlbumQueries, AlbumQueries>();
        services.AddScoped<IAlbumMembership, AlbumMembership>();
        services.AddScoped<IAlbumNames, AlbumNames>();
        services.AddScoped<IAlbumMessaging, AlbumMessaging>();
        services.AddScoped<IAlbumSuccession, AlbumSuccession>();
        services.AddScoped<CreateAlbumHandler>();
        services.AddScoped<CreateAlbumFromPeriodHandler>();
        services.AddScoped<InviteToAlbumHandler>();
        services.AddScoped<AnswerInvitationHandler>();
        services.AddScoped<WithdrawInvitationHandler>();
        services.AddScoped<ContributionWithdrawal>();
        services.AddScoped<AddMemoryToAlbumHandler>();
        services.AddScoped<AddPhotoInAlbumHandler>();
        services.AddScoped<LeaveAlbumHandler>();
        services.AddScoped<RemoveParticipantHandler>();
        services.AddScoped<RevokeKeptContributionsHandler>();
        services.AddScoped<EntrustToCuratorHandler>();
        services.AddScoped<RevokeEntrustmentHandler>();
        services.AddScoped<ArrangeEntrustedHandler>();
        services.AddScoped<TagParticipantHandler>();
        services.AddScoped<RemoveOwnTagHandler>();
        services.AddScoped<RecordInvitationWeekPassed>();
        services.AddScoped<WithdrawContributionsWhenAccountDeleted>();
        services.AddScoped<ForgetContentWhenDeleted>();
        services.AddScoped<TrimAlbumWhenOrdered>();
        services.AddScoped<RestoreTrimmedWhenPlanActive>();
        services.AddScoped<RemoveBlockedParticipant>();
        services.AddScoped<ReserveTakeoverHandler>();
        services.AddScoped<ExpireTakeoverReservation>();
        services.AddScoped<TakeOverWhenPaid>();

        // IDeadlines exists in the worker only, the one host running these (D-157): built on demand, so the
        // API, which never asks for them, still passes the container's validation in development.
        services.AddScoped(provider =>
            new InvitationReminderDeadline(provider.GetRequiredService<IAlbums>(), provider.GetRequiredService<IDeadlines>()));
        services.AddScheduledJob<InvitationReminderJob>(InvitationReminderDeadline.JobName);
        services.AddScoped(provider => new TakeoverReservationDeadline(provider.GetRequiredService<IDeadlines>()));
        services.AddScheduledJob<TakeoverReservationJob>(TakeoverReservationDeadline.JobName);

        services.AddHealthChecks().AddCheck<AlbumDatabaseHealthCheck>(AlbumDatabaseHealthCheck.Name, tags: [HealthTags.Ready]);
        if (configuration.GetValue("Database:MigrateOnStartup", defaultValue: true))
        {
            services.AddHostedService<AlbumDatabaseMigrator>();
        }
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints.MapAlbumsEndpoints();
}
