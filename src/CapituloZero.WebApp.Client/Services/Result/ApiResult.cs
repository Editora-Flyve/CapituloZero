namespace CapituloZero.WebApp.Client.Services.Result;

public sealed record ApiResult<T>(bool Success, T? Data, string? ErrorCode, string? ErrorMessage)
{
    public static ApiResult<T> Ok(T data) => new(true, data, null, null);
    public static ApiResult<T> Fail(string code, string? message) => new(false, default, code, message);
}
