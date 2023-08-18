namespace ApiCommons.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string representation of a number to its equivalent integer representation.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <returns>An integer representation of the string value. If the string value is null or empty, returns the default value for int (0).</returns>
        public static int ToInt(this string value)
        {
            return string.IsNullOrEmpty(value) ? default : Convert.ToInt32(value);
        }

        /// <summary>
        /// Converts a string representation of a number to its equivalent long integer representation.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <returns>A long integer representation of the string value. If the string value is null or empty, returns the default value for long (0).</returns>
        public static long ToLong(this string value)
        {
            return string.IsNullOrEmpty(value) ? default : Convert.ToInt64(value);
        }

        /// <summary>
        /// Converts a string representation of a number to its equivalent decimal representation.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <returns>A decimal representation of the string value. If the string value is null or empty, returns the default value for decimal (0).</returns>
        public static decimal ToDecimal(this string value)
        {
            return string.IsNullOrEmpty(value) ? default : Convert.ToDecimal(value);
        }

        /// <summary>
        /// Converts a string representation of a date and time to its equivalent <see cref="DateTime"/> representation.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <returns>A <see cref="DateTime"/> representation of the string value. If the string value is null or empty, returns the default value for <see cref="DateTime"/>.</returns>
        public static DateTime ToDateTime(this string value)
        {
            if (string.IsNullOrEmpty(value)) return default;
            return Convert.ToDateTime(value);
        }

        /// <summary>
        /// Converts a string representation of a number to its equivalent double-precision floating-point representation.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <returns>A double-precision floating-point representation of the string value. If the string value is null or empty, returns the default value for double (0).</returns>
        public static double ToDouble(this string value)
        {
            if (string.IsNullOrEmpty(value)) return default;
            return Convert.ToDouble(value);
        }
    }
}
