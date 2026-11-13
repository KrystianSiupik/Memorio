using Memorio.Modularity;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Archive.Api;
using Memorio.Modules.Archive.Application.Commands.AddPhoto;
using Memorio.Modules.Archive.Application.Commands.ApproveDescription;
using Memorio.Modules.Archive.Application.Commands.CreateMemory;
using Memorio.Modules.Archive.Application.Commands.DeleteAccountContent;
using Memorio.Modules.Archive.Application.Commands.DeleteMemory;
using Memorio.Modules.Archive.Application.Commands.RestoreMemory;
using Memorio.Modules.Archive.Application.Commands.TrashMemory;
using Memorio.Modules.Archive.Application.Commands.TrimArchive;
using Memorio.Modules.Archive.Application.Policies;
using Memorio.Modules.Archive.Application.Queries.MemoryDetails;
using Memorio.Modules.Archive.Application.Queries.MemoryMap;
using Memorio.Modules.Archive.Application.Queries.MyMemories;
using Memorio.Modules.Archive.Application.Queries.Period;
using Memorio.Modules.Archive.Application.Queries.Trash;
using Memorio.Modules.Archive.Application.Queries.TrimMaterial;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Archive.Domain.Memories;
using Memorio.Modules.Archive.Infrastructure;
using Memorio.Modules.Archive.Infrastructure.Persistence;
using Memorio.Modules.Archive.Infrastructure.Queries;
using Memorio.Modules.Archive.Infrastructure.Scheduling;
using Memorio.SharedKernel.Messaging;
using Memorio.SharedKernel.Scheduling;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wolverine.EntityFrameworkCore;

namespace Memorio.Modules.Archive;

/// <summary>Module Archive. Key question: What did this person record, where and when?</summary>
public sealed class ArchiveModule : IModule
{
    public string Name => "Archive";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ArchiveOptions>()
            .Bind(configuration.GetSection(ArchiveOptions.Section))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "Archive__ConnectionString is required.")
            .ValidateOnStart();

        // A missing value fails the validation above at startup; the placeholder only lets the host be built.
        var connectionString = configuration.GetSection(ArchiveOptions.Section).Get<ArchiveOptions>()?.ConnectionString;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=unconfigured";
        }

        services.TryAddSingleton(TimeProvider.System);
        services.AddDbContextWithWolverineIntegration<ArchiveDbContext>(database => database.UseArchiveDatabase(connectionString));
        services.AddScoped<IAccountContentReader, AccountContentReader>();
        services.AddScoped<IAlbumContent, AlbumContent>();
        services.AddScoped<IMemoryFacts, MemoryFactsQuery>();
        services.AddScoped<IMemories, Memories>();
        services.AddScoped<DeleteAccountContentHandler>();
        services.AddScoped<CreateMemoryHandler>();
        services.AddScoped<IMemoryMapQuery, MemoryMapQuery>();
        services.AddScoped<IMyMemoriesQuery, MyMemoriesQuery>();
        services.AddScoped<DeleteMemoryWhenTrashWindowPassed>();

        // IDeadlines exists in the worker only, the one host running these (D-157): built on demand, so the
        // API, which never asks for them, still passes the container's validation in development.
        services.AddScoped(provider => new TrashWindowDeadline(provider.GetRequiredService<IMemories>(), provider.GetRequiredService<IDeadlines>()));
        services.AddScoped<IIntegrationEventPolicy<MemoryMovedToTrashEvent>>(provider => provider.GetRequiredService<TrashWindowDeadline>());
        services.AddScoped<IIntegrationEventPolicy<MemoryRestoredEvent>>(provider => provider.GetRequiredService<TrashWindowDeadline>());
        services.AddScheduledJob<TrashWindowJob>(TrashWindowDeadline.JobName);
        services.AddScheduledJob<TrashSweepJob>(TrashSweepJob.JobName, TrashSweepJob.Cron);
        services.AddScoped<RestoreMemoryHandler>();
        services.AddScoped<ITrimMaterialQuery, TrimMaterialQuery>();
        services.AddScoped<IPeriodQuery, PeriodQuery>();
        services.AddScoped<TrimArchiveHandler>();
        services.AddScoped<RestoreTrimmedHandler>();
        services.AddScoped<TrimWhenOrdered>();
        services.AddScoped<RestoreTrimmedWhenPlanActive>();
        services.AddScoped<ITrashQuery, TrashQuery>();
        services.AddScoped<RequestDataDeletionHandler>();
        services.AddScoped<MemoryEraser>();
        services.AddScoped<EraseAwaitingHandler>();
        services.AddScoped<DeleteMemoryHandler>();
        services.AddScoped<TrashMemoryHandler>();
        services.AddScoped<MemoryDetailsQuery>();
        services.AddScoped<IMemoryDetailsQuery>(provider => provider.GetRequiredService<MemoryDetailsQuery>());
        services.AddScoped<IReportedMemories>(provider => provider.GetRequiredService<MemoryDetailsQuery>());
        services.AddScoped<ApproveDescriptionHandler>();
        services.AddScoped<AddPhotoHandler>();
        services.AddScoped<StartUploadHandler>();
        services.AddScoped<DeleteContentWhenAccountDeleted>();

        services.AddHealthChecks().AddCheck<ArchiveDatabaseHealthCheck>(ArchiveDatabaseHealthCheck.Name, tags: [HealthTags.Ready]);
        if (configuration.GetValue("Database:MigrateOnStartup", defaultValue: true))
        {
            services.AddHostedService<ArchiveDatabaseMigrator>();
        }
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints) => endpoints.MapMemoriesEndpoints();
}
