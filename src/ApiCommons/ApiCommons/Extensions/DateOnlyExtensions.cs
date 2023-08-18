namespace ApiCommons.Extensions
{
    public static class DateOnlyExtensions
    {
        /// <summary>
        /// Converts a <see cref="DateOnly"/> value to a <see cref="DateTime"/> value with the time component set to midnight (00:00:00).
        /// </summary>
        /// <param name="date">The <see cref="DateOnly"/> value to convert.</param>
        /// <returns>A <see cref="DateTime"/> value representing the date portion of the input <see cref="DateOnly"/> value with the time component set to midnight.</returns>
        public static DateTime ToDateTime(this DateOnly date)
        {
            return date.ToDateTime(new TimeOnly()).Date;
        }
    }
}
