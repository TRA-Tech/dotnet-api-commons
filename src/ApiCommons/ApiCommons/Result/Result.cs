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
        /// Gets the success value of the result, if present. This property is null when the result represents a failure.
        /// </summary>
        public TValue? Value { get { return _value; } }

        /// <summary>
        /// Gets the error value of the result, if present. This property is null when the result represents a success.
        /// </summary>
        public TError? Error { get { return _error; } }

        /// <summary>
        /// Gets a value indicating whether the result is an error. Returns true if the result contains an error, otherwise false.
        /// </summary>
        public bool IsError { get => _error is not null; }

        /// <summary>
        /// Gets a value indicating whether the result is a success. Returns true if the result is successful and contains a value, otherwise false.
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
        /// Matches the result with the appropriate function based on its state and returns the outcome.
        /// Ideal for handling operations with distinct success or failure actions.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="success">Function to execute if the result is a success.</param>
        /// <param name="failure">Function to execute if the result is a failure.</param>
        /// <returns>The result of the executed function.</returns>
        public TResult Match<TResult>(
            Func<TValue, TResult> success,
            Func<TError, TResult> failure)
        {
            return IsSuccess ? success(_value!) : failure(_error!);
        }

        /// <summary>
        /// Asynchronously matches the result with the appropriate async function based on its state, returning a task of the result.
        /// Useful for async operations needing distinct handling for success or failure.
        /// </summary>
        /// <typeparam name="TResult">The type of the async result.</typeparam>
        /// <param name="success">Async function to execute if the result is successful.</param>
        /// <param name="failure">Async function to execute if the result is a failure.</param>
        /// <returns>A ValueTask of the executed function's result.</returns>
        public ValueTask<TResult> MatchAsync<TResult>(
            Func<TValue, ValueTask<TResult>> success,
            Func<TError, ValueTask<TResult>> failure)
        {
            return IsSuccess ? success(_value!) : failure(_error!);
        }

        /// <summary>
        /// Executes an action based on the result's state. Useful for side effects like logging.
        /// </summary>
        /// <param name="success">Action to execute if the result is successful, using the success value.</param>
        /// <param name="failure">Action to execute if the result is a failure, using the error value.</param>
        public void Handle(
            Action<TValue> success,
            Action<TError> failure)
        {
            if (IsSuccess)
                success(_value!);
            else
                failure(_error!);
        }

        /// <summary>
        /// Asynchronously executes an action based on the result's state. Ideal for handling side effects that involve async operations.
        /// </summary>
        /// <param name="success">Async action to execute if the result is successful, using the success value.</param>
        /// <param name="failure">Async action to execute if the result is a failure, using the error value.</param>
        /// <returns>A ValueTask representing the ongoing operation.</returns>
        public ValueTask HandleAsync(
            Func<TValue, ValueTask> success,
            Func<TError, ValueTask> failure)
        {
            return IsSuccess ? success(_value!) : failure(_error!);
        }
    }

}
