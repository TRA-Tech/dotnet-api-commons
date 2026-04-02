using System.IO.Compression;
using System.Text;

namespace ApiCommons.Helpers;

/// <summary>
/// Provides Brotli compression and decompression helpers.
/// Intended for non-latency-critical use cases such as cache value compression and queue payloads.
/// </summary>
public static class BrotliHelper
{
    /// <summary>
    /// Brotli-compresses a byte array.
    /// </summary>
    /// <param name="input">The raw bytes to compress.</param>
    /// <param name="level">The compression level. Defaults to <see cref="CompressionLevel.Optimal"/>.</param>
    /// <returns>The compressed bytes.</returns>
    public static byte[] Compress(byte[] input, CompressionLevel level = CompressionLevel.Optimal)
    {
        ArgumentNullException.ThrowIfNull(input);
        using var output = new MemoryStream();
        using (var brotli = new BrotliStream(output, level, leaveOpen: true))
            brotli.Write(input);
        return output.ToArray();
    }

    /// <summary>
    /// Encodes a string as UTF-8, Brotli-compresses it, and returns the result as a Base64 string.
    /// Use <see cref="Decompress(string)"/> to reverse.
    /// </summary>
    /// <param name="input">The string to compress.</param>
    /// <param name="level">The compression level. Defaults to <see cref="CompressionLevel.Optimal"/>.</param>
    /// <returns>A Base64-encoded compressed string.</returns>
    public static string Compress(string input, CompressionLevel level = CompressionLevel.Optimal)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(input);
        return Convert.ToBase64String(Compress(Encoding.UTF8.GetBytes(input), level));
    }

    /// <summary>
    /// Brotli-decompresses a byte array previously compressed with <see cref="Compress(byte[], CompressionLevel)"/>.
    /// </summary>
    /// <param name="input">The compressed bytes to decompress.</param>
    /// <returns>The original uncompressed bytes.</returns>
    public static byte[] Decompress(byte[] input)
    {
        ArgumentNullException.ThrowIfNull(input);
        using var output = new MemoryStream();
        using var brotli = new BrotliStream(new MemoryStream(input), CompressionMode.Decompress);
        brotli.CopyTo(output);
        return output.ToArray();
    }

    /// <summary>
    /// Decodes a Base64 string, Brotli-decompresses it, and returns the result as a UTF-8 string.
    /// Reverses <see cref="Compress(string, CompressionLevel)"/>.
    /// </summary>
    /// <param name="input">The Base64-encoded compressed string to decompress.</param>
    /// <returns>The original uncompressed string.</returns>
    public static string Decompress(string input)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(input);
        return Encoding.UTF8.GetString(Decompress(Convert.FromBase64String(input)));
    }
}
