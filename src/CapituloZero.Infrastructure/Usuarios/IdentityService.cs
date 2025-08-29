using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Application.Users.Login;
using CapituloZero.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CapituloZero.SharedKernel;
using CapituloZero.Domain.Users;
using CapituloZero.Infrastructure.Users;

namespace CapituloZero.Infrastructure.Usuarios;

internal sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ITokenProvider tokenProvider,
    ApplicationDbContext dbContext)
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
            return Result.Failure<Guid>(ErrorInternal.Problem("Users.CreateFailed", string.Join(';', created.Errors.Select(e => e.Description))));
        }

        // Ensure Default role exists
        if (!await roleManager.RoleExistsAsync(UserTypes.Default).ConfigureAwait(false))
        {
            await roleManager.CreateAsync(new ApplicationRole { Id = Guid.NewGuid(), Name = UserTypes.Default, NormalizedName = UserTypes.Default.ToUpperInvariant() }).ConfigureAwait(false);
        }
        await userManager.AddToRoleAsync(user, UserTypes.Default).ConfigureAwait(false);

        return user.Id;
    }

    public async Task<Result<LoginResponse>> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await userManager.Users.SingleOrDefaultAsync(u => u.Email == email, ct).ConfigureAwait(false);
        if (user is null)
        {
            return Result.Failure<LoginResponse>(ErrorInternal.NotFound("Users.NotFoundByEmail", "User not found"));
        }
        if (!await userManager.CheckPasswordAsync(user, password).ConfigureAwait(false))
        {
            return Result.Failure<LoginResponse>(ErrorInternal.Problem("Users.InvalidCredentials", "Invalid email or password"));
        }

        var roles = await userManager.GetRolesAsync(user).ConfigureAwait(false);
        var accessToken = tokenProvider.Create(user.Id, user.Email!, roles);

        // Revoga refresh tokens antigos ativos para este usuário
        var activeTokens = await dbContext.RefreshTokens
            .Where(rt => rt.UserId == user.Id && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(ct)
            .ConfigureAwait(false);
        if (activeTokens.Count > 0)
        {
            foreach (var t in activeTokens)
            {
                t.RevokedAt = DateTime.UtcNow;
            }
        }

        // Geração e persistência do novo refresh token
        var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
        dbContext.RefreshTokens.Add(refreshTokenEntity);
        await dbContext.SaveChangesAsync(ct).ConfigureAwait(false);

        return new LoginResponse(accessToken, refreshToken);
    }

    public async Task<Result<CapituloZero.Application.Users.GetById.UserResponse>> GetByIdAsync(Guid id, Guid currentUserId, CancellationToken ct = default)
    {
        if (id != currentUserId)
        {
            return Result.Failure<CapituloZero.Application.Users.GetById.UserResponse>(ErrorInternal.Problem("Users.Unauthorized", "You cannot access another user."));
        }
        var user = await userManager.Users.Where(u => u.Id == id).Select(u => new { u.Id, u.FirstName, u.LastName, u.Email }).SingleOrDefaultAsync(ct).ConfigureAwait(false);
        if (user is null)
        {
            return Result.Failure<CapituloZero.Application.Users.GetById.UserResponse>(ErrorInternal.NotFound("Users.NotFound", "User not found"));
        }
        var entity = await userManager.FindByIdAsync(user.Id.ToString()).ConfigureAwait(false);
        if (entity is null)
        {
            return Result.Failure<CapituloZero.Application.Users.GetById.UserResponse>(ErrorInternal.NotFound("Users.NotFound", "User not found"));
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
            return Result.Failure<CapituloZero.Application.Users.GetByEmail.UserResponse>(ErrorInternal.NotFound("Users.NotFoundByEmail", "User not found"));
        }
        if (user.Id != currentUserId)
        {
            return Result.Failure<CapituloZero.Application.Users.GetByEmail.UserResponse>(ErrorInternal.Problem("Users.Unauthorized", "You cannot access another user."));
        }
        var entity = await userManager.FindByIdAsync(user.Id.ToString()).ConfigureAwait(false);
        if (entity is null)
        {
            return Result.Failure<CapituloZero.Application.Users.GetByEmail.UserResponse>(ErrorInternal.NotFound("Users.NotFoundByEmail", "User not found"));
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
            return Result.Failure(ErrorInternal.NotFound("Users.NotFound", "User not found"));
        }
        var distinct = tipos.Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        foreach (var tipo in distinct)
        {
            if (!UserTypes.Allowed.Contains(tipo))
            {
                return Result.Failure(ErrorInternal.Problem("Users.InvalidType", $"Invalid type: {tipo}"));
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
                return Result.Failure(ErrorInternal.Problem("Users.AssignRolesFailed", string.Join(';', result.Errors.Select(e => e.Description))));
            }
        }
        return Result.Success();
    }

    public async Task<Result<LoginResponse>> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var tokenEntity = await dbContext.RefreshTokens
            .AsTracking()
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow, ct)
            .ConfigureAwait(false);

        if (tokenEntity is null)
        {
            return Result.Failure<LoginResponse>(ErrorInternal.Problem("Auth.InvalidRefreshToken", "Refresh token inválido ou expirado."));
        }

        var user = await userManager.FindByIdAsync(tokenEntity.UserId.ToString()).ConfigureAwait(false);
        if (user is null)
        {
            return Result.Failure<LoginResponse>(ErrorInternal.NotFound("Users.NotFound", "Usuário não encontrado."));
        }

        // Revoga o token antigo
        tokenEntity.RevokedAt = DateTime.UtcNow;

        var roles = await userManager.GetRolesAsync(user).ConfigureAwait(false);
        var accessToken = tokenProvider.Create(user.Id, user.Email!, roles);

        // Cria novo refresh token
        var newRefresh = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var newEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefresh,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
        dbContext.RefreshTokens.Add(newEntity);

        await dbContext.SaveChangesAsync(ct).ConfigureAwait(false);

        return new LoginResponse(accessToken, newRefresh);
    }

    public async Task<Result<IReadOnlyList<CapituloZero.Application.Users.Get.UserListItemResponse>>> GetAllAsync(CancellationToken ct = default)
    {
        var users = await userManager.Users
            .AsNoTracking()
            .Select(u => new { u.Id, u.Email })
            .ToListAsync(ct)
            .ConfigureAwait(false);

        var responses = new List<CapituloZero.Application.Users.Get.UserListItemResponse>(users.Count);
        foreach (var u in users)
        {
            var entity = await userManager.FindByIdAsync(u.Id.ToString()).ConfigureAwait(false);
            if (entity is null)
            {
                continue;
            }
            var roles = await userManager.GetRolesAsync(entity).ConfigureAwait(false);
            responses.Add(new CapituloZero.Application.Users.Get.UserListItemResponse
            {
                Id = u.Id,
                Email = u.Email!,
                Tipos = roles.ToList()
            });
        }

        return responses;
    }
}
