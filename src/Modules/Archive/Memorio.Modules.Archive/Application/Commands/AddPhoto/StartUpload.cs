using Memorio.Modules.Archive.Domain.Memories;
using Memorio.SharedKernel.Results;
using Memorio.SharedKernel.Storage;

namespace Memorio.Modules.Archive.Application.Commands.AddPhoto;

/// <summary>An address to send a photo to; it becomes part of a memory only through <see cref="AddPhoto"/>.</summary>
internal sealed class StartUploadHandler(IObjectStorage storage)
{
    private static readonly string[] Accepted = ["image/jpeg", "image/png", "image/webp", "image/heic"];

    public async Task<Result<UploadSlot>> HandleAsync(string? contentType, CancellationToken cancellationToken) =>
        contentType is not null && Accepted.Contains(contentType, StringComparer.Ordinal)
            ? await storage.CreateUploadAsync(contentType, cancellationToken)
            : MemoryErrors.InvalidContentType;
}
