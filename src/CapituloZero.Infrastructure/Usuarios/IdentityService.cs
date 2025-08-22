using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Users.GetByEmail;
using CapituloZero.Application.Users.GetById;
using CapituloZero.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CapituloZero.SharedKernel;

namespace CapituloZero.Infrastructure.Usuarios;

internal sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ITokenProvider tokenProvider)
    : IIdentityService
{
    public async Task<Result<Guid>> RegisterAsync(string email, string firstName, string lastName, string password, CancellationToken ct = default)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = false
        };

        var created = await userManager.CreateAsync(user, password).ConfigureAwait(false);
        if (!created.Succeeded)
        {
            return Result.Failure<Guid>(Error.Problem("Usuarios.CreateFailed", string.Join(';', created.Errors.Select(e => e.Description))));
        }

        // Ensure Default role exists
        if (!await roleManager.RoleExistsAsync(UserTypes.Default).ConfigureAwait(false))
        {
            await roleManager.CreateAsync(new ApplicationRole { Id = Guid.NewGuid(), Name = UserTypes.Default, NormalizedName = UserTypes.Default.ToUpperInvariant() }).ConfigureAwait(false);
        }
        await userManager.AddToRoleAsync(user, UserTypes.Default).ConfigureAwait(false);

        return user.Id;
    }

    public async Task<Result<string>> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await userManager.Users.SingleOrDefaultAsync(u => u.Email == email, ct).ConfigureAwait(false);
        if (user is null)
        {
            return Result.Failure<string>(Error.NotFound("Usuarios.NotFoundByEmail", "User not found"));
        }
        if (!await userManager.CheckPasswordAsync(user, password).ConfigureAwait(false))
        {
            return Result.Failure<string>(Error.Problem("Usuarios.InvalidCredentials", "Invalid email or password"));
        }

        var token = CreateToken(user);
        return token;
    }

    private string CreateToken(ApplicationUser user)
    {
        // Leverage TokenProvider’s internal Create that enriches roles via UserManager
        var tmp = new CapituloZero.Domain.Users.User { Id = user.Id, Email = user.Email!, FirstName = user.FirstName, LastName = user.LastName };
        return tokenProvider.Create(tmp);
    }

    public async Task<Result<CapituloZero.Application.Users.GetById.UserResponse>> GetByIdAsync(Guid id, Guid currentUserId, CancellationToken ct = default)
    {
        if (id != currentUserId)
        {
            return Result.Failure<CapituloZero.Application.Users.GetById.UserResponse>(Error.Problem("Usuarios.Unauthorized", "You cannot access another user."));
        }
        var user = await userManager.Users.Where(u => u.Id == id).Select(u => new CapituloZero.Application.Users.GetById.UserResponse
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email!
        }).SingleOrDefaultAsync(ct).ConfigureAwait(false);
        if (user is null)
        {
            return Result.Failure<CapituloZero.Application.Users.GetById.UserResponse>(Error.NotFound("Usuarios.NotFound", "User not found"));
        }
        return user;
    }

    public async Task<Result<CapituloZero.Application.Users.GetByEmail.UserResponse>> GetByEmailAsync(string email, Guid currentUserId, CancellationToken ct = default)
    {
        var user = await userManager.Users.Where(u => u.Email == email).Select(u => new CapituloZero.Application.Users.GetByEmail.UserResponse
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email!
        }).SingleOrDefaultAsync(ct).ConfigureAwait(false);
        if (user is null)
        {
            return Result.Failure<CapituloZero.Application.Users.GetByEmail.UserResponse>(Error.NotFound("Usuarios.NotFoundByEmail", "User not found"));
        }
        if (user.Id != currentUserId)
        {
            return Result.Failure<CapituloZero.Application.Users.GetByEmail.UserResponse>(Error.Problem("Usuarios.Unauthorized", "You cannot access another user."));
        }
        return user;
    }

    public async Task<Result> AddUserTypesAsync(Guid userId, IEnumerable<string> tipos, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
        if (user is null)
        {
            return Result.Failure(Error.NotFound("Usuarios.NotFound", "User not found"));
        }
        var distinct = tipos.Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        foreach (var tipo in distinct)
        {
            if (!UserTypes.Allowed.Contains(tipo))
            {
                return Result.Failure(Error.Problem("Usuarios.TipoInvalido", $"Tipo inválido: {tipo}"));
            }
            if (!await roleManager.RoleExistsAsync(tipo).ConfigureAwait(false))
            {
                await roleManager.CreateAsync(new ApplicationRole { Id = Guid.NewGuid(), Name = tipo, NormalizedName = tipo.ToUpperInvariant() }).ConfigureAwait(false);
            }
        }
        // Always ensure Default role assignment
        if (!await userManager.IsInRoleAsync(user, UserTypes.Default).ConfigureAwait(false))
        {
            await userManager.AddToRoleAsync(user, UserTypes.Default).ConfigureAwait(false);
        }
        var toAssign = distinct.Where(t => !t.Equals(UserTypes.Default, StringComparison.OrdinalIgnoreCase)).ToList();
        if (toAssign.Count > 0)
        {
            var result = await userManager.AddToRolesAsync(user, toAssign).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return Result.Failure(Error.Problem("Usuarios.AssignRolesFailed", string.Join(';', result.Errors.Select(e => e.Description))));
            }
        }
        return Result.Success();
    }
}
