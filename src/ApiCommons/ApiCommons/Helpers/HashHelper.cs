using System.Security.Cryptography;
using System.Text;

namespace ApiCommons.Helpers;

/// <summary>
/// Provides hashing helpers.
/// For non-security uses only (cache keys, checksums, ETags).
/// </summary>
public static class HashHelper
{
    /// <summary>
    /// Returns a lowercase MD5 hex string for the given input using UTF-8 encoding.
    /// </summary>
    public static string Md5(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);
        byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
