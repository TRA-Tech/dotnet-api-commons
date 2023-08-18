namespace ApiCommons.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns the current date and time with an additional 3 hours.
        /// </summary>
        /// <param name="value">The input date and time value.</param>
        /// <returns>A <see cref="DateTime"/> value representing the current date and time with an additional 3 hours.</returns>
        public static DateTime DateNow(this DateTime value)
        {
            return value.AddHours(3);
        }

        /// <summary>
        /// Determines whether a given date is a weekday (Monday to Friday).
        /// </summary>
        /// <param name="date">The date to check.</param>
        /// <returns>True if the date is a weekday; otherwise, false.</returns>
        public static bool IsWeekday(this DateTime date)
        {
            return !(date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
        }

        /// <summary>
        /// Determines whether a given date is a weekend (Saturday or Sunday).
        /// </summary>
        /// <param name="date">The date to check.</param>
        /// <returns>True if the date is a weekend; otherwise, false.</returns>
        public static bool IsWeekend(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> value to a <see cref="DateOnly"/> value.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> value to convert.</param>
        /// <returns>A <see cref="DateOnly"/> value representing the date portion of the input <see cref="DateTime"/> value.</returns>
        public static DateOnly ToDateOnly(this DateTime date)
        {
            return new DateOnly(date.Year, date.Month, date.Day);
        }
    }
}
