namespace CapituloZero.SharedKernel;

public sealed record ValidationErrorInternal : ErrorInternal
{
    public ValidationErrorInternal(ErrorInternal[] errors)
        : base(
            "Validation.General",
            "One or more validation errors occurred",
            ErrorType.Validation)
    {
        Errors = errors;
    }

    public ErrorInternal[] Errors { get; }

    public static ValidationErrorInternal FromResults(IEnumerable<Result> results) =>
        new(results.Where(r => r.IsFailure).Select(r => r.ErrorInternal).ToArray());
}
