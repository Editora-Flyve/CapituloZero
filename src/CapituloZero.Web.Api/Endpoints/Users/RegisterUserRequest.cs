using CapituloZero.Domain.Users;

namespace CapituloZero.Web.Api.Endpoints.Users;

internal sealed record RegisterUserRequest(
    string Email,
    string FirstName,
    string LastName,
    string Password,
    UserType? Types);
