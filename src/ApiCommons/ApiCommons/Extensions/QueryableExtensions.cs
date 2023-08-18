using System.Linq.Expressions;

namespace ApiCommons.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Filters a sequence of elements based on a specified condition if the condition is met.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">The source sequence to filter elements from.</param>
        /// <param name="condition">The condition that determines whether to filter elements or not.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An <see cref="IQueryable{TSource}"/> that contains elements from the input sequence that satisfy the condition if the condition is true; otherwise, returns the original source sequence.</returns>
        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }

        /// <summary>
        /// Takes a specified number of elements from the beginning of a sequence if a condition is met.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">The source sequence to take elements from.</param>
        /// <param name="condition">The condition that determines whether to take elements or not.</param>
        /// <param name="count">The number of elements to take if the condition is true.</param>
        /// <returns>An <see cref="IQueryable{TSource}"/> that takes the specified number of elements if the condition is true; otherwise, returns the original source sequence.</returns>
        public static IQueryable<TSource> TakeIf<TSource>(this IQueryable<TSource> source, bool condition, int count)
        {
            return condition ? source.Take(count) : source;
        }

        /// <summary>
        /// Skips a specified number of elements from the beginning of a sequence if a condition is met.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">The source sequence to skip elements from.</param>
        /// <param name="condition">The condition that determines whether to skip elements or not.</param>
        /// <param name="count">The number of elements to skip if the condition is true.</param>
        /// <returns>An <see cref="IQueryable{TSource}"/> that skips the specified number of elements if the condition is true; otherwise, returns the original source sequence.</returns>
        public static IQueryable<TSource> SkipIf<TSource>(this IQueryable<TSource> source, bool condition, int count)
        {
            return condition ? source.Skip(count) : source;
        }

        /// <summary>
        /// Asynchronously creates a <see cref="HashSet{TSource}"/> from an <see cref="IQueryable{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">The source sequence to create the <see cref="HashSet{TSource}"/> from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="HashSet{TSource}"/> that contains unique elements from the input sequence.</returns>
        public static async Task<HashSet<TSource>> ToHashSetAsync<TSource>(this IQueryable<TSource> source)
        {
            return await Task.Run(() =>
            {
                return source.ToHashSet();
            });
        }
    }
}
