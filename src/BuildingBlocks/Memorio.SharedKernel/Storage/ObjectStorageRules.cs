namespace Memorio.SharedKernel.Storage;

/// <summary>Rules of the media storage (ADR 0005, SD5, QS4). They belong to the requirements, not to configuration.</summary>
public static class ObjectStorageRules
{
    /// <summary>
    /// How long a signed address works: access withdrawn in the Access module stops working for media
    /// within this time (QS4: media ≤ 5 min). SD5: never more than 300 s.
    /// </summary>
    public static readonly TimeSpan SignatureLifetime = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Temporary upload zone: the app uploads here with a signed address; a module moves the object to
    /// its final key once it accepted it. The bucket expires anything left here after a day.
    /// </summary>
    public const string UploadPrefix = "uploads/";
}
