using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Users.GetByEmail;
using CapituloZero.Application.Users.GetById;
using CapituloZero.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CapituloZero.SharedKernel;

namespace CapituloZero.Infrastructure.Users;

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
            return Result.Failure<Guid>(Error.Problem("Users.CreateFailed", string.Join(';', created.Errors.Select(e => e.Description))));
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
            return Result.Failure<string>(Error.NotFound("Users.NotFoundByEmail", "User not found"));
        }
        if (!await userManager.CheckPasswordAsync(user, password).ConfigureAwait(false))
        {
            return Result.Failure<string>(Error.Problem("Users.InvalidCredentials", "Invalid email or password"));
        }

    var roles = await userManager.GetRolesAsync(user).ConfigureAwait(false);
    var token = tokenProvider.Create(user.Id, user.Email!, roles);
        return token;
    }

    public async Task<Result<CapituloZero.Application.Users.GetById.UserResponse>> GetByIdAsync(Guid id, Guid currentUserId, CancellationToken ct = default)
    {
        if (id != currentUserId)
        {
            return Result.Failure<CapituloZero.Application.Users.GetById.UserResponse>(Error.Problem("Users.Unauthorized", "You cannot access another user."));
        }
        var user = await userManager.Users.Where(u => u.Id == id).Select(u => new { u.Id, u.FirstName, u.LastName, u.Email }).SingleOrDefaultAsync(ct).ConfigureAwait(false);
        if (user is null)
        {
            return Result.Failure<CapituloZero.Application.Users.GetById.UserResponse>(Error.NotFound("Users.NotFound", "User not found"));
        }
        var entity = await userManager.FindByIdAsync(user.Id.ToString()).ConfigureAwait(false);
        if (entity is null)
        {
            return Result.Failure<CapituloZero.Application.Users.GetById.UserResponse>(Error.NotFound("Users.NotFound", "User not found"));
        }
        var roles = await userManager.GetRolesAsync(entity).ConfigureAwait(false);
        var response = new CapituloZero.Application.Users.GetById.UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            Tipos = roles.ToList()
        };
        return response;
    }

    public async Task<Result<CapituloZero.Application.Users.GetByEmail.UserResponse>> GetByEmailAsync(string email, Guid currentUserId, CancellationToken ct = default)
    {
        var user = await userManager.Users.Where(u => u.Email == email).Select(u => new { u.Id, u.FirstName, u.LastName, u.Email }).SingleOrDefaultAsync(ct).ConfigureAwait(false);
        if (user is null)
        {
            return Result.Failure<CapituloZero.Application.Users.GetByEmail.UserResponse>(Error.NotFound("Users.NotFoundByEmail", "User not found"));
        }
        if (user.Id != currentUserId)
        {
            return Result.Failure<CapituloZero.Application.Users.GetByEmail.UserResponse>(Error.Problem("Users.Unauthorized", "You cannot access another user."));
        }
        var entity = await userManager.FindByIdAsync(user.Id.ToString()).ConfigureAwait(false);
        if (entity is null)
        {
            return Result.Failure<CapituloZero.Application.Users.GetByEmail.UserResponse>(Error.NotFound("Users.NotFoundByEmail", "User not found"));
        }
        var roles = await userManager.GetRolesAsync(entity).ConfigureAwait(false);
        var response = new CapituloZero.Application.Users.GetByEmail.UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            Tipos = roles.ToList()
        };
        return response;
    }

    public async Task<Result> AddUserTypesAsync(Guid userId, IEnumerable<string> tipos, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);
        if (user is null)
        {
            return Result.Failure(Error.NotFound("Users.NotFound", "User not found"));
        }
        var distinct = tipos.Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        foreach (var tipo in distinct)
        {
            if (!UserTypes.Allowed.Contains(tipo))
            {
                return Result.Failure(Error.Problem("Users.InvalidType", $"Invalid type: {tipo}"));
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
                return Result.Failure(Error.Problem("Users.AssignRolesFailed", string.Join(';', result.Errors.Select(e => e.Description))));
            }
        }
        return Result.Success();
    }
}
