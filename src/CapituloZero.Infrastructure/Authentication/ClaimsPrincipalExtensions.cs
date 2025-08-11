using System.Security.Claims;
using CapituloZero.Domain.Users;

namespace CapituloZero.Infrastructure.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userId, out Guid parsedUserId)
            ? parsedUserId
            : throw new InvalidOperationException("User id is unavailable");
    }

    public static string? GetActiveType(this ClaimsPrincipal? principal)
        => principal?.FindFirst("active_type")?.Value;
}
