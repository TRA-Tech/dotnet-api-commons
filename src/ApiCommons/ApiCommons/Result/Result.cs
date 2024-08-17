namespace ApiCommons.Result
{
    /// <summary>
    /// Represents a result that can either be a success containing a value or a failure containing an error.
    /// </summary>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    public readonly struct Result<TValue, TError>
    {
        private readonly TValue? _value;
        private readonly TError? _error;

        /// <summary>
        /// Gets a value indicating whether the result is an error.
        /// </summary>
        public bool IsError { get => _error is null; }

        /// <summary>
        /// Gets a value indicating whether the result is a success.
        /// </summary>
        public bool IsSuccess { get => !IsError; }

        /// <summary>
        /// Implicitly converts a success value to a <see cref="Result{TValue, TError}"/>.
        /// </summary>
        /// <param name="value">The success value.</param>
        /// <returns>A <see cref="Result{TValue, TError}"/> representing a successful result.</returns>
        public static implicit operator Result<TValue, TError>(TValue value) => new(value);

        /// <summary>
        /// Implicitly converts an error to a <see cref="Result{TValue, TError}"/>.
        /// </summary>
        /// <param name="error">The error value.</param>
        /// <returns>A <see cref="Result{TValue, TError}"/> representing a failed result.</returns>
        public static implicit operator Result<TValue, TError>(TError error) => new(error);

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TValue, TError}"/> struct with a success value.
        /// </summary>
        /// <param name="value">The success value.</param>
        private Result(TValue value)
        {
            _value = value;
            _error = default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TValue, TError}"/> struct with an error.
        /// </summary>
        /// <param name="error">The error value.</param>
        private Result(TError error)
        {
            _error = error;
            _value = default;
        }

        /// <summary>
        /// Matches the result with a function for success and a function for failure, and returns the result of the matched function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="success">The function to call if the result is a success.</param>
        /// <param name="failure">The function to call if the result is a failure.</param>
        /// <returns>The result of the matched function.</returns>
        public TResult Match<TResult>(
            Func<TValue?, TResult> success,
            Func<TError, TResult> failure)
        {
            return IsError ? success(_value) : failure(_error!);
        }
    }

}
