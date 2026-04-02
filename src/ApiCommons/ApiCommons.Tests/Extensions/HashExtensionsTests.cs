using ApiCommons.Helpers;
using FluentAssertions;
using Xunit;

namespace ApiCommons.Tests.Extensions;

public class HashHelperTests
{
    // Known MD5 hash: MD5("hello") = "5d41402abc4b2a76b9719d911017c592"
    [Fact]
    public void Md5_KnownInput_ReturnsExpectedLowercaseHex()
    {
        HashHelper.Md5("hello").Should().Be("5d41402abc4b2a76b9719d911017c592");
    }

    [Fact]
    public void Md5_OutputIsLowercase()
    {
        var hash = HashHelper.Md5("test");
        hash.Should().Be(hash.ToLowerInvariant());
    }

    [Fact]
    public void Md5_SameInput_ProducesSameHash()
    {
        HashHelper.Md5("ApiCommons").Should().Be(HashHelper.Md5("ApiCommons"));
    }

    [Fact]
    public void Md5_DifferentInputs_ProduceDifferentHashes()
    {
        HashHelper.Md5("hello").Should().NotBe(HashHelper.Md5("world"));
    }

    [Fact]
    public void Md5_EmptyString_ThrowsArgumentException()
    {
        Action act = () => HashHelper.Md5("");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Md5_NullString_ThrowsArgumentException()
    {
        Action act = () => HashHelper.Md5(null!);
        act.Should().Throw<ArgumentException>();
    }
}
