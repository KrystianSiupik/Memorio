using Memorio.Modularity;
using Memorio.Modules.Access;
using Memorio.Modules.Account;
using Memorio.Modules.AiNarration;
using Memorio.Modules.Album;
using Memorio.Modules.Archive;
using Memorio.Modules.Limits;
using Memorio.Modules.Moderation;
using Memorio.Modules.Notifications;
using Memorio.Modules.PlaceProfiles;
using Memorio.Modules.Places;
using Memorio.Modules.Plans;
using Memorio.Modules.Provenance;
using Memorio.Modules.Publishing;
using Memorio.Modules.Succession;

namespace Memorio.Api;

/// <summary>
/// Composition root of the modules: the only place that knows every module. A module that is not
/// listed here is not part of the application.
/// </summary>
internal static class ModuleCatalog
{
    public static IReadOnlyList<IModule> All { get; } =
    [
        new ArchiveModule(),
        new AlbumModule(),
        new AccessModule(),
        new ProvenanceModule(),
        new PlacesModule(),
        new LimitsModule(),
        new PlansModule(),
        new AiNarrationModule(),
        new ModerationModule(),
        new AccountModule(),
        new SuccessionModule(),
        new NotificationsModule(),
        new PlaceProfilesModule(),
        new PublishingModule(),
    ];
}
