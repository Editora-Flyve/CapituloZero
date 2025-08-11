using CapituloZero.Application.Abstractions.Data;
using CapituloZero.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CapituloZero.Infrastructure.Authorization;

internal static class BuiltinPermissions
{
    public const string UsersAccess = "users:access";
    public const string AdminAccess = "admin:access";
    public const string TerceirosAccess = "terceiros:access";
    public const string AutorAccess = "autor:access";
}

internal sealed class PermissionProvider(IApplicationDbContext context)
{
    public async Task<HashSet<string>> GetForUserIdAsync(Guid userId)
    {
        var user = await context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId).ConfigureAwait(false);
        if (user is null)
        {
            return [];
        }

    var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { BuiltinPermissions.UsersAccess };

        if (user.Types.HasFlag(UserType.Admin))
        {
            // Admin has full access
            permissions.Add(BuiltinPermissions.AdminAccess);
            permissions.Add(BuiltinPermissions.TerceirosAccess);
            permissions.Add(BuiltinPermissions.AutorAccess);
        }
        if (user.Types.HasFlag(UserType.Terceiro))
        {
            permissions.Add(BuiltinPermissions.TerceirosAccess);
        }
        if (user.Types.HasFlag(UserType.Autor))
        {
            permissions.Add(BuiltinPermissions.AutorAccess);
            // Author may collaborate in terceiros area (kanban submissions)
            permissions.Add(BuiltinPermissions.TerceirosAccess);
        }

        // Cliente type implies ecommerce access (future); keeping UsersAccess only for now.

        return permissions;
    }
}
