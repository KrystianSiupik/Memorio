using System.Security.Cryptography;

namespace Memorio.Modules.Limits.Contracts;

/// <summary>A reservation key that is always the same for one pair: one content in one album counts once (D-015).</summary>
public static class PoolKeys
{
    public static Guid For(Guid first, Guid second)
    {
        Span<byte> bytes = stackalloc byte[32];
        first.TryWriteBytes(bytes);
        second.TryWriteBytes(bytes[16..]);
        return new Guid(SHA256.HashData(bytes).AsSpan(0, 16));
    }
}
