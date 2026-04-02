using System.Security.Claims;
using ApiCommons.Extensions;
using FluentAssertions;
using Xunit;

namespace ApiCommons.Tests.Extensions;

public class ClaimsPrincipalExtensionsTests
{
    private static ClaimsPrincipal UserWith(string type, string value) =>
        new(new ClaimsIdentity([new Claim(type, value)], "test"));

    private static ClaimsPrincipal EmptyUser() =>
        new(new ClaimsIdentity());

    // ── GetId<int> ────────────────────────────────────────────────────────────

    [Fact]
    public void GetId_Int_WhenValidClaim_ParsesCorrectly()
    {
        var user = UserWith(ClaimTypes.NameIdentifier, "42");
        user.GetId<int>().Should().Be(42);
    }

    [Fact]
    public void GetId_Int_WhenClaimAbsent_ReturnsDefault()
    {
        EmptyUser().GetId<int>().Should().Be(default);
    }

    [Fact]
    public void GetId_Int_WhenClaimUnparseable_ReturnsDefault()
    {
        var user = UserWith(ClaimTypes.NameIdentifier, "not-a-number");
        user.GetId<int>().Should().Be(default);
    }

    // ── GetId<Guid> ───────────────────────────────────────────────────────────

    [Fact]
    public void GetId_Guid_WhenValidClaim_ParsesCorrectly()
    {
        var id = Guid.NewGuid();
        var user = UserWith(ClaimTypes.NameIdentifier, id.ToString());
        user.GetId<Guid>().Should().Be(id);
    }

    [Fact]
    public void GetId_Guid_WhenClaimAbsent_ReturnsDefault()
    {
        EmptyUser().GetId<Guid>().Should().Be((Guid)default);
    }

    // ── GetId<string> — covers former GetSubject ──────────────────────────────

    [Fact]
    public void GetId_String_ReturnsRawSubjectString()
    {
        var user = UserWith(ClaimTypes.NameIdentifier, "sub-abc-123");
        user.GetId<string>().Should().Be("sub-abc-123");
    }

    [Fact]
    public void GetId_String_WhenClaimAbsent_ReturnsNull()
    {
        EmptyUser().GetId<string>().Should().BeNull();
    }

    // ── GetRequiredId<int> ────────────────────────────────────────────────────

    [Fact]
    public void GetRequiredId_WhenValidClaim_ReturnsValue()
    {
        var user = UserWith(ClaimTypes.NameIdentifier, "7");
        user.GetRequiredId<int>().Should().Be(7);
    }

    [Fact]
    public void GetRequiredId_WhenClaimMissing_ThrowsInvalidOperationException()
    {
        Action act = () => EmptyUser().GetRequiredId<int>();
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetRequiredId_WhenClaimUnparseable_ThrowsInvalidOperationException()
    {
        var user = UserWith(ClaimTypes.NameIdentifier, "not-a-number");
        Action act = () => user.GetRequiredId<int>();
        act.Should().Throw<InvalidOperationException>();
    }

    // ── GetEmail ──────────────────────────────────────────────────────────────

    [Fact]
    public void GetEmail_WhenEmailClaimPresent_ReturnsEmail()
    {
        var user = UserWith(ClaimTypes.Email, "user@example.com");
        user.GetEmail().Should().Be("user@example.com");
    }

    [Fact]
    public void GetEmail_WhenEmailClaimAbsent_ReturnsNull()
    {
        EmptyUser().GetEmail().Should().BeNull();
    }
}
