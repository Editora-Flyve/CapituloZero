using System.Security.Claims;

namespace CapituloZero.Infrastructure.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal? principal)
    {
        // Tenta obter pelo NameIdentifier; se não houver mapeamento, usa o "sub"
        string? userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier) ??
                         principal?.FindFirstValue("sub");

        return Guid.TryParse(userId, out Guid parsedUserId) ?
            parsedUserId :
            throw new InvalidOperationException("User id is unavailable");
    }
}
