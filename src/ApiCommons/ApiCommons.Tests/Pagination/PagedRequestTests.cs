using ApiCommons.Pagination;
using FluentAssertions;
using Xunit;

namespace ApiCommons.Tests.Pagination;

public class PagedRequestTests
{
    // ── PageIndex normalization ────────────────────────────────────────────────

    [Fact]
    public void PageIndex_Negative_ClampsToZero()
    {
        var req = new PagedRequest(-5, 20);
        req.PageIndex.Should().Be(0);
    }

    [Fact]
    public void PageIndex_Zero_RemainsZero()
    {
        var req = new PagedRequest(0, 20);
        req.PageIndex.Should().Be(0);
    }

    [Fact]
    public void PageIndex_Positive_IsPreserved()
    {
        var req = new PagedRequest(3, 20);
        req.PageIndex.Should().Be(3);
    }

    // ── PageSize normalization ────────────────────────────────────────────────

    [Fact]
    public void PageSize_Zero_ClampsToOne()
    {
        var req = new PagedRequest(0, 0);
        req.PageSize.Should().Be(1);
    }

    [Fact]
    public void PageSize_Negative_ClampsToOne()
    {
        var req = new PagedRequest(0, -10);
        req.PageSize.Should().Be(1);
    }

    [Fact]
    public void PageSize_AboveMaxPageSize_ClampsToMax()
    {
        var req = new PagedRequest(0, 500, 200);
        req.PageSize.Should().Be(200);
    }

    [Fact]
    public void PageSize_ExactlyMaxPageSize_IsPreserved()
    {
        var req = new PagedRequest(0, 200, 200);
        req.PageSize.Should().Be(200);
    }

    [Fact]
    public void PageSize_WithinBounds_IsPreserved()
    {
        var req = new PagedRequest(0, 50);
        req.PageSize.Should().Be(50);
    }

    // ── MaxPageSize normalization ─────────────────────────────────────────────

    [Fact]
    public void MaxPageSize_Zero_ClampsToOne()
    {
        var req = new PagedRequest(0, 10, 0);
        req.MaxPageSize.Should().Be(1);
    }

    // ── Skip ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Skip_IsPageIndexTimesPageSize()
    {
        var req = new PagedRequest(3, 10);
        req.Skip.Should().Be(30);
    }

    [Fact]
    public void Skip_WhenPageIndexIsZero_IsZero()
    {
        var req = new PagedRequest(0, 20);
        req.Skip.Should().Be(0);
    }

    // ── Default constructor ───────────────────────────────────────────────────

    [Fact]
    public void DefaultConstructor_SetsExpectedDefaults()
    {
        var req = new PagedRequest();
        req.PageIndex.Should().Be(0);
        req.PageSize.Should().Be(20);
        req.MaxPageSize.Should().Be(200);
    }
}
