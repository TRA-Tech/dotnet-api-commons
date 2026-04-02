namespace ApiCommons.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Returns <see langword="true"/> if the date falls on a weekday (Monday through Friday).
    /// </summary>
    public static bool IsWeekday(this DateTime date)
        => date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday;

    /// <summary>
    /// Returns <see langword="true"/> if the date falls on a weekend (Saturday or Sunday).
    /// </summary>
    public static bool IsWeekend(this DateTime date)
        => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    /// <summary>
    /// Extracts the date portion of a <see cref="DateTime"/> as a <see cref="DateOnly"/> value.
    /// </summary>
    public static DateOnly ToDateOnly(this DateTime date)
        => new(date.Year, date.Month, date.Day);
}
