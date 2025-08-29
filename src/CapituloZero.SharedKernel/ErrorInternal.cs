namespace CapituloZero.SharedKernel;

public record ErrorInternal
{
    public static readonly ErrorInternal None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly ErrorInternal NullValue = new(
        "General.Null",
        "Null value was provided",
        ErrorType.Failure);

    public ErrorInternal(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    public string Code { get; }

    public string Description { get; }

    public ErrorType Type { get; }

    public static ErrorInternal Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static ErrorInternal NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static ErrorInternal Problem(string code, string description) =>
        new(code, description, ErrorType.Problem);

    public static ErrorInternal Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);
}
