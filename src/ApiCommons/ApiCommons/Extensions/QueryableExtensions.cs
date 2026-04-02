using System.Linq.Expressions;
using System.Reflection;
using ApiCommons.Pagination;
using Microsoft.EntityFrameworkCore;

namespace ApiCommons.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// Filters the sequence using <paramref name="predicate"/> only when <paramref name="condition"/> is
    /// <see langword="true"/>; otherwise returns the source query unchanged.
    /// </summary>
    public static IQueryable<TSource> WhereIf<TSource>(
        this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        => condition ? source.Where(predicate) : source;

    /// <summary>
    /// Takes the first <paramref name="count"/> elements only when <paramref name="condition"/> is
    /// <see langword="true"/>; otherwise returns the source query unchanged.
    /// </summary>
    public static IQueryable<TSource> TakeIf<TSource>(
        this IQueryable<TSource> source, bool condition, int count)
        => condition ? source.Take(count) : source;

    /// <summary>
    /// Skips the first <paramref name="count"/> elements only when <paramref name="condition"/> is
    /// <see langword="true"/>; otherwise returns the source query unchanged.
    /// </summary>
    public static IQueryable<TSource> SkipIf<TSource>(
        this IQueryable<TSource> source, bool condition, int count)
        => condition ? source.Skip(count) : source;

    /// <summary>
    /// Materializes a single page of results from an <see cref="IQueryable{TSource}"/>.
    /// Executes two queries against the underlying provider: <c>CountAsync</c> for the total,
    /// then a paged <c>ToListAsync</c> using <see cref="PagedRequest.Skip"/> and <see cref="PagedRequest.PageSize"/>.
    /// </summary>
    /// <param name="source">The queryable source. Apply filters and projections before calling.</param>
    /// <param name="request">The normalized pagination request.</param>
    /// <param name="ct">Cancellation token.</param>
    public static async Task<PagedResult<TSource>> ToPagedAsync<TSource>(
        this IQueryable<TSource> source,
        PagedRequest request,
        CancellationToken ct = default)
    {
        var total = await source.CountAsync(ct);
        if (total == 0) return PagedResult<TSource>.Empty(request);

        var items = await source
            .Skip(request.Skip)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return PagedResult<TSource>.From(items, total, request);
    }

    /// <summary>
    /// Materializes a single page of results, applying dynamic multi-column sorting from
    /// <see cref="SortedPagedRequest.Sorting"/> before paging.
    /// Sorting is built via expression trees and reflection so no additional NuGet dependency is required.
    /// When <see cref="SortedPagedRequest.Sorting"/> is empty no <c>OrderBy</c> clause is added.
    /// Unknown property names in <see cref="SortColumn.Id"/> are silently ignored.
    /// </summary>
    /// <param name="source">The queryable source. Apply filters and projections before calling.</param>
    /// <param name="request">The pagination request including sort columns.</param>
    /// <param name="ct">Cancellation token.</param>
    public static async Task<PagedResult<TSource>> ToPagedAsync<TSource>(
        this IQueryable<TSource> source,
        SortedPagedRequest request,
        CancellationToken ct = default)
    {
        if (request.Sorting.Count > 0)
            source = ApplySorting(source, request.Sorting);

        var total = await source.CountAsync(ct);
        if (total == 0) return PagedResult<TSource>.Empty(request);

        var items = await source
            .Skip(request.Skip)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return PagedResult<TSource>.From(items, total, request);
    }

    private static IQueryable<TSource> ApplySorting<TSource>(
        IQueryable<TSource> source,
        IReadOnlyList<SortColumn> sorting)
    {
        IOrderedQueryable<TSource>? ordered = null;
        var type = typeof(TSource);
        var queryableMethods = typeof(Queryable).GetMethods();

        foreach (var col in sorting)
        {
            var prop = type.GetProperty(
                col.Id,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (prop is null) continue;

            var param = Expression.Parameter(type, "x");
            var body = Expression.Property(param, prop);
            var keySelector = Expression.Lambda(body, param);

            var methodName = ordered is null
                ? (col.Desc ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy))
                : (col.Desc ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy));

            var method = queryableMethods
                .FirstOrDefault(m => m.Name == methodName && m.GetParameters().Length == 2)
                ?? throw new InvalidOperationException(
                    $"Could not find Queryable.{methodName} with 2 parameters.");

            ordered = (IOrderedQueryable<TSource>)method
                .MakeGenericMethod(type, prop.PropertyType)
                .Invoke(null, [(object)(ordered ?? source), keySelector])!;
        }

        return ordered ?? source;
    }
}
