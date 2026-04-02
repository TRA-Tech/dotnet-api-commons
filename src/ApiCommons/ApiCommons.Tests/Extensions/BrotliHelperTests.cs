using ApiCommons.Helpers;
using FluentAssertions;
using Xunit;

namespace ApiCommons.Tests.Extensions;

public class BrotliHelperTests
{
    // ── byte[] round-trip ─────────────────────────────────────────────────────

    [Fact]
    public void Compress_Decompress_ByteArray_RoundTrip()
    {
        var original = "Hello, ApiCommons v2!"u8.ToArray();
        var compressed = BrotliHelper.Compress(original);
        var restored = BrotliHelper.Decompress(compressed);
        restored.Should().Equal(original);
    }

    [Fact]
    public void Compress_ByteArray_ProducesDifferentBytes()
    {
        var original = "Some data to compress"u8.ToArray();
        var compressed = BrotliHelper.Compress(original);
        compressed.Should().NotEqual(original);
    }

    [Fact]
    public void Compress_ByteArray_NullInput_ThrowsArgumentNullException()
    {
        Action act = () => BrotliHelper.Compress((byte[])null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Decompress_ByteArray_NullInput_ThrowsArgumentNullException()
    {
        Action act = () => BrotliHelper.Decompress((byte[])null!);
        act.Should().Throw<ArgumentNullException>();
    }

    // ── string round-trip ─────────────────────────────────────────────────────

    [Fact]
    public void Compress_Decompress_String_RoundTrip()
    {
        const string original = "Hello, ApiCommons v2! This is a test string for Brotli compression.";
        var compressed = BrotliHelper.Compress(original);
        var restored = BrotliHelper.Decompress(compressed);
        restored.Should().Be(original);
    }

    [Fact]
    public void Compress_String_ReturnsBase64String()
    {
        var compressed = BrotliHelper.Compress("test");
        Action act = () => Convert.FromBase64String(compressed);
        act.Should().NotThrow();
    }

    [Fact]
    public void Compress_String_NullInput_ThrowsArgumentNullException()
    {
        Action act = () => BrotliHelper.Compress((string)null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Compress_String_EmptyInput_ThrowsArgumentException()
    {
        Action act = () => BrotliHelper.Compress("");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Decompress_String_NullInput_ThrowsArgumentNullException()
    {
        Action act = () => BrotliHelper.Decompress((string)null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Decompress_String_EmptyInput_ThrowsArgumentException()
    {
        Action act = () => BrotliHelper.Decompress("");
        act.Should().Throw<ArgumentException>();
    }
}
