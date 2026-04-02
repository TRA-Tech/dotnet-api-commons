using ApiCommons.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ApiCommons.Tests.Extensions;

public class ConfigurationExtensionsTests
{
    private static IConfiguration BuildConfig(Dictionary<string, string?> values) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();

    [Fact]
    public void GetRequired_WhenKeyExists_ReturnsValue()
    {
        var config = BuildConfig(new() { ["MyKey"] = "hello" });
        config.GetRequired("MyKey").Should().Be("hello");
    }

    [Fact]
    public void GetRequired_WhenKeyMissing_ThrowsInvalidOperationException()
    {
        var config = BuildConfig([]);
        Action act = () => config.GetRequired("Missing");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Missing*");
    }

    [Fact]
    public void GetRequired_WhenValueIsNull_ThrowsInvalidOperationException()
    {
        var config = BuildConfig(new() { ["NullKey"] = null });
        Action act = () => config.GetRequired("NullKey");
        act.Should().Throw<InvalidOperationException>();
    }
}
