using ApiCommons.Pagination;
using FluentAssertions;
using Xunit;

namespace ApiCommons.Tests.Pagination;

public class PagedResultTests
{
    // ── TotalPages edge cases ─────────────────────────────────────────────────

    [Fact]
    public void TotalPages_ZeroItems_ReturnsZero()
    {
        var req = new PagedRequest(0, 10);
        var result = PagedResult<int>.Empty(req);
        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public void TotalPages_ItemsExactlyDivisibleByPageSize_ReturnsExactCount()
    {
        var req = new PagedRequest(0, 10);
        var result = new PagedResult<int>([], 30, req);
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public void TotalPages_ItemsNotExactlyDivisible_RoundsUp()
    {
        var req = new PagedRequest(0, 10);
        var result = new PagedResult<int>([], 25, req);
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public void TotalPages_OneItem_ReturnsOne()
    {
        var req = new PagedRequest(0, 10);
        var result = new PagedResult<int>([1], 1, req);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public void TotalPages_OneLessThanPageSize_ReturnsOne()
    {
        var req = new PagedRequest(0, 10);
        var result = new PagedResult<int>([], 9, req);
        result.TotalPages.Should().Be(1);
    }

    // ── Metadata propagation ──────────────────────────────────────────────────

    [Fact]
    public void PagedResult_PropagatesPageIndexAndPageSize_FromRequest()
    {
        var req = new PagedRequest(2, 15);
        var result = new PagedResult<string>([], 100, req);
        result.PageIndex.Should().Be(2);
        result.PageSize.Should().Be(15);
        result.TotalCount.Should().Be(100);
    }

    [Fact]
    public void Empty_ProducesZeroItemsAndZeroTotalCount()
    {
        var req = new PagedRequest(0, 20);
        var result = PagedResult<int>.Empty(req);
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public void From_FactoryMethod_ProducesSameResultAsConstructor()
    {
        var req = new PagedRequest(1, 10);
        var items = new[] { 1, 2, 3 };
        var viaConstructor = new PagedResult<int>(items, 13, req);
        var viaFactory = PagedResult<int>.From(items, 13, req);
        viaFactory.Items.Should().BeEquivalentTo(viaConstructor.Items);
        viaFactory.TotalCount.Should().Be(viaConstructor.TotalCount);
        viaFactory.PageIndex.Should().Be(viaConstructor.PageIndex);
        viaFactory.PageSize.Should().Be(viaConstructor.PageSize);
    }
}
