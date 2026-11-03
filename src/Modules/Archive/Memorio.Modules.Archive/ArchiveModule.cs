using Memorio.Modularity;
using Memorio.Modules.Account.Contracts;
using Memorio.Modules.Archive.Application.Commands.DeleteAccountContent;
using Memorio.Modules.Archive.Application.Policies;
using Memorio.Modules.Archive.Contracts;
using Memorio.Modules.Archive.Domain.Memories;
using Memorio.Modules.Archive.Infrastructure;
using Memorio.Modules.Archive.Infrastructure.Persistence;
using Memorio.Modules.Archive.Infrastructure.Queries;
using Memorio.SharedKernel.Messaging;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        services.AddDbContext<ArchiveDbContext>(database => database.UseArchiveDatabase(connectionString));
        services.AddScoped<IAccountContentReader, AccountContentReader>();
        services.AddScoped<IMemories, Memories>();
        services.AddScoped<DeleteAccountContentHandler>();
        services.AddScoped<IIntegrationEventPolicy<AccountDeleted>, DeleteContentWhenAccountDeleted>();

        services.AddHealthChecks().AddCheck<ArchiveDatabaseHealthCheck>(ArchiveDatabaseHealthCheck.Name, tags: [HealthTags.Ready]);
        if (configuration.GetValue("Database:MigrateOnStartup", defaultValue: true))
        {
            services.AddHostedService<ArchiveDatabaseMigrator>();
        }
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
