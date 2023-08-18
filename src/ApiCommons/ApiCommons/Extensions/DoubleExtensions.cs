namespace ApiCommons.Extensions
{
    public static class DoubleExtensions
    {
        /// <summary>
        /// Converts a double-precision floating-point number to its equivalent integer representation.
        /// </summary>
        /// <param name="value">The double value to convert.</param>
        /// <returns>An integer representation of the double value.</returns>
        public static int ToInt(this double value)
        {
            return Convert.ToInt32(value);
        }
    }
}
