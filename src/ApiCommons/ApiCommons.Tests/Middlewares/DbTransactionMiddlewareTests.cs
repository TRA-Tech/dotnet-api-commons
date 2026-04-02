using ApiCommons.Attributes;
using ApiCommons.Middlewares.DbTransaction;
using ApiCommons.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ApiCommons.Tests.Middlewares;

/// <summary>
/// Integration tests for <see cref="DbTransactionMiddleware"/>.
/// Uses SQLite in-memory (via a shared, kept-open connection) to verify real transaction commit/rollback.
/// A <see cref="CommitTracker"/> interceptor records which path was taken.
/// </summary>
public sealed class DbTransactionMiddlewareTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly CommitTracker _tracker;
    private readonly TestDbContext _db;

    public DbTransactionMiddlewareTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _tracker = new CommitTracker();
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(_connection)
            .AddInterceptors(_tracker)
            .Options;

        _db = new TestDbContext(options);
        _db.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private DefaultHttpContext BuildHttpContext()
    {
        var httpContext = new DefaultHttpContext();

        var endpoint = new Endpoint(
            requestDelegate: null,
            metadata: new EndpointMetadataCollection(new DbTransactionAttribute(typeof(TestDbContext))),
            displayName: "test");
        httpContext.SetEndpoint(endpoint);

        var services = new ServiceCollection();
        services.AddSingleton(_db);
        httpContext.RequestServices = services.BuildServiceProvider();

        return httpContext;
    }

    // ── Tests ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Invoke_WhenNextSucceeds_CommitsTransaction()
    {
        var httpContext = BuildHttpContext();
        var middleware = new DbTransactionMiddleware(async _ =>
        {
            _db.Items.Add(new TestItem { Id = 1, Name = "committed", SortOrder = 1 });
            await _db.SaveChangesAsync();
        });

        await middleware.Invoke(httpContext);

        _tracker.Committed.Should().BeTrue("next() completed without throwing");
        _tracker.RolledBack.Should().BeFalse();
    }

    [Fact]
    public async Task Invoke_WhenNextThrows_RollsBackTransaction_AndRethrows()
    {
        var httpContext = BuildHttpContext();
        var middleware = new DbTransactionMiddleware(_ => throw new InvalidOperationException("fail"));

        Func<Task> act = () => middleware.Invoke(httpContext);

        await act.Should().ThrowAsync<InvalidOperationException>("the exception must propagate to the caller");
        _tracker.RolledBack.Should().BeTrue("an exception must trigger rollback");
        _tracker.Committed.Should().BeFalse();
    }

    [Fact]
    public async Task Invoke_WhenNoDbTransactionAttribute_CallsNextWithoutTransaction()
    {
        // Endpoint has no DbTransactionAttribute → middleware skips the transaction block
        var httpContext = new DefaultHttpContext();
        var nextCalled = false;
        var middleware = new DbTransactionMiddleware(_ => { nextCalled = true; return Task.CompletedTask; });

        await middleware.Invoke(httpContext);

        nextCalled.Should().BeTrue();
        _tracker.Committed.Should().BeFalse("no transaction should have been started");
        _tracker.RolledBack.Should().BeFalse();
    }

    [Fact]
    public async Task Invoke_WhenDbContextNotRegistered_CallsNextWithoutTransaction()
    {
        // Endpoint has attribute but service provider does not have the DbContext
        var httpContext = new DefaultHttpContext();
        var endpoint = new Endpoint(
            requestDelegate: null,
            metadata: new EndpointMetadataCollection(new DbTransactionAttribute(typeof(TestDbContext))),
            displayName: "test");
        httpContext.SetEndpoint(endpoint);
        httpContext.RequestServices = new ServiceCollection().BuildServiceProvider(); // empty

        var nextCalled = false;
        var middleware = new DbTransactionMiddleware(_ => { nextCalled = true; return Task.CompletedTask; });

        await middleware.Invoke(httpContext);

        nextCalled.Should().BeTrue();
        _tracker.Committed.Should().BeFalse();
    }
}
