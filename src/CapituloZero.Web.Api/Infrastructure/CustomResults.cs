using CapituloZero.SharedKernel;

namespace CapituloZero.Web.Api.Infrastructure;

public static class CustomResults
{
    public static IResult Problem(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException();
        }

        return Results.Problem(
            title: GetTitle(result.ErrorInternal),
            detail: GetDetail(result.ErrorInternal),
            type: GetType(result.ErrorInternal.Type),
            statusCode: GetStatusCode(result.ErrorInternal.Type),
            extensions: GetErrors(result));

        static string GetTitle(ErrorInternal errorInternal) =>
            errorInternal.Type switch
            {
                ErrorType.Validation => errorInternal.Code,
                ErrorType.Problem => errorInternal.Code,
                ErrorType.NotFound => errorInternal.Code,
                ErrorType.Conflict => errorInternal.Code,
                _ => "Server failure"
            };

        static string GetDetail(ErrorInternal errorInternal) =>
            errorInternal.Type switch
            {
                ErrorType.Validation => errorInternal.Description,
                ErrorType.Problem => errorInternal.Description,
                ErrorType.NotFound => errorInternal.Description,
                ErrorType.Conflict => errorInternal.Description,
                _ => "An unexpected errorInternal occurred"
            };

        static string GetType(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                ErrorType.Problem => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };

        static int GetStatusCode(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.Validation or ErrorType.Problem => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

        static Dictionary<string, object?>? GetErrors(Result result)
        {
            if (result.ErrorInternal is not ValidationErrorInternal validationError)
            {
                return null;
            }

            return new Dictionary<string, object?>
            {
                { "errors", validationError.Errors }
            };
        }
    }
}
