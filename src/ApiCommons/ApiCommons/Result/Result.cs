namespace ApiCommons.Result;

/// <summary>
/// Represents a discriminated union that is either a success containing <typeparamref name="TValue"/>
/// or a failure containing <typeparamref name="TError"/>.
/// Use the <see cref="Result{TValue}"/> shorthand when the error type is always <see cref="Error"/>.
/// </summary>
/// <typeparam name="TValue">The type of the success value.</typeparam>
/// <typeparam name="TError">The type of the error.</typeparam>
public readonly struct Result<TValue, TError>
{
    private readonly TValue? _value;
    private readonly TError? _error;

    /// <summary>The success value. Only valid when <see cref="IsSuccess"/> is <see langword="true"/>.</summary>
    public TValue? Value => _value;

    /// <summary>The error value. Only valid when <see cref="IsError"/> is <see langword="true"/>.</summary>
    public TError? Error => _error;

    /// <summary>Returns <see langword="true"/> when this result represents a failure.</summary>
    public bool IsError => _error is not null;

    /// <summary>Returns <see langword="true"/> when this result represents a success.</summary>
    public bool IsSuccess => !IsError;

    /// <summary>Implicitly wraps a success value into a <see cref="Result{TValue, TError}"/>.</summary>
    public static implicit operator Result<TValue, TError>(TValue value) => new(value);

    /// <summary>Implicitly wraps an error into a <see cref="Result{TValue, TError}"/>.</summary>
    public static implicit operator Result<TValue, TError>(TError error) => new(error);

    private Result(TValue value)
    {
        _value = value;
        _error = default;
    }

    private Result(TError error)
    {
        _error = error;
        _value = default;
    }

    /// <summary>
    /// Projects the result to <typeparamref name="TResult"/> by invoking <paramref name="success"/>
    /// on the value or <paramref name="failure"/> on the error.
    /// </summary>
    public TResult Match<TResult>(Func<TValue, TResult> success, Func<TError, TResult> failure)
        => IsSuccess ? success(_value!) : failure(_error!);

    /// <summary>
    /// Asynchronously projects the result to <typeparamref name="TResult"/> by invoking
    /// <paramref name="success"/> on the value or <paramref name="failure"/> on the error.
    /// </summary>
    public ValueTask<TResult> MatchAsync<TResult>(
        Func<TValue, ValueTask<TResult>> success,
        Func<TError, ValueTask<TResult>> failure)
        => IsSuccess ? success(_value!) : failure(_error!);

    /// <summary>
    /// Executes <paramref name="success"/> when the result is successful,
    /// or <paramref name="failure"/> when it is a failure. Useful for side-effects such as logging.
    /// </summary>
    public void Handle(Action<TValue> success, Action<TError> failure)
    {
        if (IsSuccess) success(_value!);
        else failure(_error!);
    }

    /// <summary>
    /// Asynchronously executes <paramref name="success"/> when the result is successful,
    /// or <paramref name="failure"/> when it is a failure.
    /// </summary>
    public ValueTask HandleAsync(Func<TValue, ValueTask> success, Func<TError, ValueTask> failure)
        => IsSuccess ? success(_value!) : failure(_error!);

    /// <summary>
    /// Transforms the success value with <paramref name="mapper"/>, preserving the error unchanged.
    /// Returns a failure result immediately if this result is already a failure.
    /// </summary>
    public Result<TNew, TError> Map<TNew>(Func<TValue, TNew> mapper)
    {
        if (IsError) return _error!;
        return mapper(_value!);
    }

    /// <summary>
    /// Invokes <paramref name="action"/> on the success value and returns the original result unchanged.
    /// The action is not called when this result is a failure.
    /// </summary>
    public Result<TValue, TError> Tap(Action<TValue> action)
    {
        if (IsSuccess) action(_value!);
        return this;
    }

    /// <summary>
    /// Chains another <see cref="Result{TNew, TError}"/>-returning operation.
    /// Short-circuits and returns the current failure if this result is already a failure.
    /// </summary>
    public Result<TNew, TError> Bind<TNew>(Func<TValue, Result<TNew, TError>> binder)
    {
        if (IsError) return _error!;
        return binder(_value!);
    }

    /// <summary>
    /// Returns the success value, or <paramref name="fallback"/> if this result is a failure.
    /// </summary>
    public TValue GetValueOrDefault(TValue fallback)
        => IsSuccess ? _value! : fallback;
}

/// <summary>
/// Shorthand for <see cref="Result{TValue, TError}"/> with <see cref="Error"/> fixed as the error type.
/// Prefer this form in application code; use <see cref="Result{TValue, TError}"/> only when a custom error type is needed.
/// </summary>
/// <typeparam name="TValue">The type of the success value.</typeparam>
public readonly struct Result<TValue>
{
    private readonly TValue? _value;
    private readonly Error? _error;

    /// <summary>The success value. Only valid when <see cref="IsSuccess"/> is <see langword="true"/>.</summary>
    public TValue? Value => _value;

    /// <summary>The error value. Only valid when <see cref="IsError"/> is <see langword="true"/>.</summary>
    public Error? Error => _error;

    /// <summary>Returns <see langword="true"/> when this result represents a failure.</summary>
    public bool IsError => _error is not null;

    /// <summary>Returns <see langword="true"/> when this result represents a success.</summary>
    public bool IsSuccess => !IsError;

    /// <summary>Implicitly wraps a success value into a <see cref="Result{TValue}"/>.</summary>
    public static implicit operator Result<TValue>(TValue value) => new(value);

    /// <summary>Implicitly wraps an <see cref="Error"/> into a <see cref="Result{TValue}"/>.</summary>
    public static implicit operator Result<TValue>(Error error) => new(error);

    /// <summary>Widens a <see cref="Result{TValue}"/> to the equivalent <see cref="Result{TValue, TError}"/> form.</summary>
    public static implicit operator Result<TValue, Error>(Result<TValue> r)
    {
        if (r.IsError) return r._error!;
        return r._value!;
    }

    private Result(TValue value)
    {
        _value = value;
        _error = default;
    }

    private Result(Error error)
    {
        _error = error;
        _value = default;
    }

    /// <summary>
    /// Projects the result to <typeparamref name="TResult"/> by invoking <paramref name="success"/>
    /// on the value or <paramref name="failure"/> on the error.
    /// </summary>
    public TResult Match<TResult>(Func<TValue, TResult> success, Func<Error, TResult> failure)
        => IsSuccess ? success(_value!) : failure(_error!);

    /// <summary>
    /// Asynchronously projects the result to <typeparamref name="TResult"/> by invoking
    /// <paramref name="success"/> on the value or <paramref name="failure"/> on the error.
    /// </summary>
    public ValueTask<TResult> MatchAsync<TResult>(
        Func<TValue, ValueTask<TResult>> success,
        Func<Error, ValueTask<TResult>> failure)
        => IsSuccess ? success(_value!) : failure(_error!);

    /// <summary>
    /// Executes <paramref name="success"/> when the result is successful,
    /// or <paramref name="failure"/> when it is a failure. Useful for side-effects such as logging.
    /// </summary>
    public void Handle(Action<TValue> success, Action<Error> failure)
    {
        if (IsSuccess) success(_value!);
        else failure(_error!);
    }

    /// <summary>
    /// Asynchronously executes <paramref name="success"/> when the result is successful,
    /// or <paramref name="failure"/> when it is a failure.
    /// </summary>
    public ValueTask HandleAsync(Func<TValue, ValueTask> success, Func<Error, ValueTask> failure)
        => IsSuccess ? success(_value!) : failure(_error!);

    /// <summary>
    /// Transforms the success value with <paramref name="mapper"/>, preserving the error unchanged.
    /// Returns a failure result immediately if this result is already a failure.
    /// </summary>
    public Result<TNew> Map<TNew>(Func<TValue, TNew> mapper)
    {
        if (IsError) return _error!;
        return mapper(_value!);
    }

    /// <summary>
    /// Invokes <paramref name="action"/> on the success value and returns the original result unchanged.
    /// The action is not called when this result is a failure.
    /// </summary>
    public Result<TValue> Tap(Action<TValue> action)
    {
        if (IsSuccess) action(_value!);
        return this;
    }

    /// <summary>
    /// Chains another <see cref="Result{TNew}"/>-returning operation.
    /// Short-circuits and returns the current failure if this result is already a failure.
    /// </summary>
    public Result<TNew> Bind<TNew>(Func<TValue, Result<TNew>> binder)
    {
        if (IsError) return _error!;
        return binder(_value!);
    }

    /// <summary>
    /// Returns the success value, or <paramref name="fallback"/> if this result is a failure.
    /// </summary>
    public TValue GetValueOrDefault(TValue fallback)
        => IsSuccess ? _value! : fallback;
}
