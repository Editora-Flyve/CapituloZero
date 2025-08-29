using System.Diagnostics.CodeAnalysis;

namespace CapituloZero.SharedKernel;

public class Result
{
    public Result(bool isSuccess, ErrorInternal errorInternal)
    {
        if (isSuccess && errorInternal != ErrorInternal.None ||
            !isSuccess && errorInternal == ErrorInternal.None)
        {
            throw new ArgumentException("Invalid errorInternal", nameof(errorInternal));
        }

        IsSuccess = isSuccess;
        ErrorInternal = errorInternal;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public ErrorInternal ErrorInternal { get; }

    public static Result Success() => new(true, ErrorInternal.None);

    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true, ErrorInternal.None);

    public static Result Failure(ErrorInternal errorInternal) => new(false, errorInternal);

    public static Result<TValue> Failure<TValue>(ErrorInternal errorInternal) =>
        new(default, false, errorInternal);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue? value, bool isSuccess, ErrorInternal errorInternal)
        : base(isSuccess, errorInternal)
    {
        _value = value;
    }

    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");

    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(ErrorInternal.NullValue);

    public static Result<TValue> ValidationFailure(ErrorInternal errorInternal) =>
        new(default, false, errorInternal);
}
