namespace CapituloZero.Infrastructure.Authorization;

using CapituloZero.Infrastructure.Users;
using Microsoft.AspNetCore.Identity;

internal sealed class PermissionProvider(UserManager<ApplicationUser> userManager)
{
    public async Task<HashSet<string>> GetForUserIdAsync(Guid userId)
    {
        HashSet<string> permissionsSet = new(StringComparer.OrdinalIgnoreCase)
        {
            // Permissão base para funcionalidades comuns de usuário
            "users:access"
        };

        ApplicationUser? user = await userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
        if (user is null)
        {
            return permissionsSet;
        }

        IList<string> roles = await userManager.GetRolesAsync(user).ConfigureAwait(false);

        // Admin: acesso total
        if (roles.Contains(UserTypes.Admin, StringComparer.OrdinalIgnoreCase))
        {
            permissionsSet.Add("users:admin");
        }

        // Autor: acesso/edição de autor
        if (roles.Contains(UserTypes.Autor, StringComparer.OrdinalIgnoreCase))
        {
            permissionsSet.Add("autor:access");
        }

        // Parceiro: acesso/edição de parceiro
        if (roles.Contains(UserTypes.Parceiro, StringComparer.OrdinalIgnoreCase))
        {
            permissionsSet.Add("parceiro:access");
        }

        return permissionsSet;
    }
}
