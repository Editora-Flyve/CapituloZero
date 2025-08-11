using CapituloZero.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CapituloZero.Infrastructure.Authentication;

internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId =>
        _httpContextAccessor
            .HttpContext?
            .User
            .GetUserId() ??
    throw new InvalidOperationException("User context is unavailable");

    public string? ActiveType =>
        _httpContextAccessor
            .HttpContext?
            .User?
            .FindFirst("active_type")?
            .Value;
}
