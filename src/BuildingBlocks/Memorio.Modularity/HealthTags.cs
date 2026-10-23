namespace Memorio.Modularity;

/// <summary>Tags of health checks a module registers (RD4).</summary>
public static class HealthTags
{
    /// <summary>The check runs on GET /ready: the host takes no traffic while it fails.</summary>
    public const string Ready = "ready";
}
