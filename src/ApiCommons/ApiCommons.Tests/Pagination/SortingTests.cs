using ApiCommons.Extensions;
using ApiCommons.Pagination;
using ApiCommons.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ApiCommons.Tests.Pagination;

/// <summary>
/// Tests for <see cref="QueryableExtensions.ToPagedAsync(IQueryable{TSource}, SortedPagedRequest, CancellationToken)"/>.
/// Uses EF Core InMemory provider — sorting is evaluated in-memory via LINQ.
/// </summary>
public sealed class SortingTests : IDisposable
{
    private readonly TestDbContext _db;

    public SortingTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new TestDbContext(options);

        _db.Items.AddRange(
            new TestItem { Id = 1, Name = "Banana",  SortOrder = 30 },
            new TestItem { Id = 2, Name = "Apple",   SortOrder = 20 },
            new TestItem { Id = 3, Name = "Cherry",  SortOrder = 10 },
            new TestItem { Id = 4, Name = "Avocado", SortOrder = 20 }
        );
        _db.SaveChanges();
    }

    public void Dispose() => _db.Dispose();

    // ── Empty Sorting ─────────────────────────────────────────────────────────

    [Fact]
    public async Task EmptySorting_ReturnsAllItems_NoOrderingApplied()
    {
        var req = new SortedPagedRequest(0, 10);  // Sorting = []
        var result = await _db.Items.ToPagedAsync(req);
        result.Items.Should().HaveCount(4);
        result.TotalCount.Should().Be(4);
    }

    // ── Single-column sort ────────────────────────────────────────────────────

    [Fact]
    public async Task SingleColumnSort_Ascending_OrdersCorrectly()
    {
        var req = new SortedPagedRequest(0, 10, [new SortColumn("Name", Desc: false)]);
        var result = await _db.Items.ToPagedAsync(req);
        result.Items.Select(i => i.Name).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task SingleColumnSort_Descending_OrdersCorrectly()
    {
        var req = new SortedPagedRequest(0, 10, [new SortColumn("Name", Desc: true)]);
        var result = await _db.Items.ToPagedAsync(req);
        result.Items.Select(i => i.Name).Should().BeInDescendingOrder();
    }

    // ── Multi-column sort ─────────────────────────────────────────────────────

    [Fact]
    public async Task MultiColumnSort_OrderByThenBy_ProducesCorrectChain()
    {
        // SortOrder ASC, then Name ASC
        // Expected: Cherry(10), Apple(20), Avocado(20), Banana(30)
        var req = new SortedPagedRequest(0, 10, [
            new SortColumn("SortOrder", Desc: false),
            new SortColumn("Name",      Desc: false)
        ]);
        var result = await _db.Items.ToPagedAsync(req);
        var names = result.Items.Select(i => i.Name).ToArray();
        names[0].Should().Be("Cherry");
        names[1].Should().Be("Apple");   // "Apple" < "Avocado" alphabetically
        names[2].Should().Be("Avocado");
        names[3].Should().Be("Banana");
    }

    [Fact]
    public async Task MultiColumnSort_ThenByDescending_ProducesCorrectChain()
    {
        // SortOrder ASC, then Name DESC
        // Expected: Cherry(10), Avocado(20), Apple(20), Banana(30)
        var req = new SortedPagedRequest(0, 10, [
            new SortColumn("SortOrder", Desc: false),
            new SortColumn("Name",      Desc: true)
        ]);
        var result = await _db.Items.ToPagedAsync(req);
        var names = result.Items.Select(i => i.Name).ToArray();
        names[0].Should().Be("Cherry");
        names[1].Should().Be("Avocado"); // "Avocado" > "Apple" desc
        names[2].Should().Be("Apple");
        names[3].Should().Be("Banana");
    }

    // ── Unknown sort column ───────────────────────────────────────────────────

    [Fact]
    public async Task UnknownSortColumn_IsIgnored_DoesNotThrow()
    {
        var req = new SortedPagedRequest(0, 10, [new SortColumn("NonExistentProperty", Desc: false)]);
        var act = async () => await _db.Items.ToPagedAsync(req);
        await act.Should().NotThrowAsync();
    }

    // ── Case-insensitive column name ──────────────────────────────────────────

    [Fact]
    public async Task SortColumnId_CaseInsensitive_MatchesProperty()
    {
        var req = new SortedPagedRequest(0, 10, [new SortColumn("name", Desc: false)]);
        var result = await _db.Items.ToPagedAsync(req);
        result.Items.Select(i => i.Name).Should().BeInAscendingOrder();
    }

    // ── Pagination with sorting ───────────────────────────────────────────────

    [Fact]
    public async Task SortedPagedRequest_Paging_ReturnsCorrectPage()
    {
        // Page 0, size 2, sorted by Name ASC
        // Full order: Apple, Avocado, Banana, Cherry
        var req = new SortedPagedRequest(0, 2, [new SortColumn("Name", Desc: false)]);
        var result = await _db.Items.ToPagedAsync(req);
        result.Items.Should().HaveCount(2);
        result.Items[0].Name.Should().Be("Apple");
        result.Items[1].Name.Should().Be("Avocado");
        result.TotalCount.Should().Be(4);
        result.TotalPages.Should().Be(2);
    }
}
