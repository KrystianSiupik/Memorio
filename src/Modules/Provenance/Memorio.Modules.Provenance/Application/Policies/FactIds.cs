using System.Security.Cryptography;

namespace Memorio.Modules.Provenance.Application.Policies;

/// <summary>One event about many memories makes one fact per memory; each fact's id is the same at every delivery.</summary>
internal static class FactIds
{
    public static Guid Of(Guid eventId, Guid memoryId)
    {
        Span<byte> input = stackalloc byte[32];
        eventId.TryWriteBytes(input[..16]);
        memoryId.TryWriteBytes(input[16..]);
        Span<byte> hash = stackalloc byte[32];
        SHA256.HashData(input, hash);
        return new Guid(hash[..16]);
    }
}
