namespace ApiCommons.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Filters the sequence using <paramref name="predicate"/> only when <paramref name="condition"/> is
    /// <see langword="true"/>; otherwise returns the source sequence unchanged.
    /// </summary>
    public static IEnumerable<TSource> WhereIf<TSource>(
        this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        => condition ? source.Where(predicate) : source;

    /// <summary>
    /// Takes the first <paramref name="count"/> elements only when <paramref name="condition"/> is
    /// <see langword="true"/>; otherwise returns the source sequence unchanged.
    /// </summary>
    public static IEnumerable<TSource> TakeIf<TSource>(
        this IEnumerable<TSource> source, bool condition, int count)
        => condition ? source.Take(count) : source;

    /// <summary>
    /// Skips the first <paramref name="count"/> elements only when <paramref name="condition"/> is
    /// <see langword="true"/>; otherwise returns the source sequence unchanged.
    /// </summary>
    public static IEnumerable<TSource> SkipIf<TSource>(
        this IEnumerable<TSource> source, bool condition, int count)
        => condition ? source.Skip(count) : source;
}
