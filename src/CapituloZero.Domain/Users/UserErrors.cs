using CapituloZero.SharedKernel;

namespace CapituloZero.Domain.Users;

public static class UserErrors
{
    public static ErrorInternal NotFound(Guid userId) => ErrorInternal.NotFound(
        "Users.NotFound",
        $"The user with the Id = '{userId}' was not found");

    public static ErrorInternal Unauthorized() => ErrorInternal.Failure(
        "Users.Unauthorized",
        "You are not authorized to perform this action.");

    public static readonly ErrorInternal NotFoundByEmail = ErrorInternal.NotFound(
        "Users.NotFoundByEmail",
        "The user with the specified email was not found");

    public static readonly ErrorInternal EmailNotUnique = ErrorInternal.Conflict(
        "Users.EmailNotUnique",
        "The provided email is not unique");
}
