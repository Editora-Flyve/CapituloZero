using CapituloZero.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace CapituloZero.Infrastructure.Authorization;

internal sealed class PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Rejeita usuários não autenticados
        if (context.User is not { Identity.IsAuthenticated: true })
        {
            return;
        }

        // 1) Verifica claims de permissão diretas no token/principal (cookbook)
        //    - Admin via claim "users:admin" libera tudo
        var claimsPermissions = context.User
            .FindAll("permission")
            .Select(c => c.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Admin por claim
        if (claimsPermissions.Contains("users:admin"))
        {
            context.Succeed(requirement);
            return;
        }

        // Permissão específica por claim
        if (claimsPermissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
            return;
        }

        // 2) Fallback: calcula permissões a partir dos roles persistidos
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        PermissionProvider permissionProvider = scope.ServiceProvider.GetRequiredService<PermissionProvider>();

        Guid userId = context.User.GetUserId();
        HashSet<string> permissions = await permissionProvider.GetForUserIdAsync(userId).ConfigureAwait(false);

        // Admin: acesso total
        if (permissions.Contains("users:admin"))
        {
            context.Succeed(requirement);
            return;
        }

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
