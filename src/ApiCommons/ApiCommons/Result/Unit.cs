namespace ApiCommons.Result;

/// <summary>
/// Represents a void return value for operations that succeed with no meaningful result.
/// Use <see cref="Value"/> as the success value in <see cref="Result{TValue}"/>.
/// </summary>
public readonly struct Unit
{
    public static readonly Unit Value = default;
}
