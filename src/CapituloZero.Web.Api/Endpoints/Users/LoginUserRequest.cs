using CapituloZero.Domain.Users;

namespace CapituloZero.Web.Api.Endpoints.Users;

internal sealed record LoginUserRequest(string Email, string Password, UserType? DesiredType);
