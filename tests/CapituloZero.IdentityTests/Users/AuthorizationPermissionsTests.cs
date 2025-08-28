using System.Security.Claims;
using CapituloZero.Infrastructure.Authorization;
using CapituloZero.Infrastructure.Database;
using CapituloZero.Infrastructure.Usuarios;
using CapituloZero.Infrastructure.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using CapituloZero.Infrastructure.DomainEvents;
using CapituloZero.SharedKernel;

namespace CapituloZero.IdentityTests.Users;

public class AuthorizationPermissionsTests
{
    private sealed class NoopDomainEventsDispatcher : IDomainEventsDispatcher
    {
        public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private static ServiceProvider BuildAuthProvider()
    {
        var services = new ServiceCollection();

        services.AddDbContext<ApplicationDbContext>(o =>
            o.UseInMemoryDatabase($"auth-permissions-tests-{Guid.NewGuid()}")
             .EnableSensitiveDataLogging());

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager();

        services.AddAuthorization();
        services.AddScoped<PermissionProvider>();
        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        // Registrar dispatcher de eventos no-op para o ApplicationDbContext
        services.AddSingleton<IDomainEventsDispatcher, NoopDomainEventsDispatcher>();

        return services.BuildServiceProvider();
    }

    private static async Task<ApplicationUser> CreateUserWithRolesAsync(ServiceProvider provider, string email, params string[] roles)
    {
        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = provider.GetRequiredService<RoleManager<ApplicationRole>>();

        foreach (var r in new[] { UserTypes.Default, UserTypes.Autor, UserTypes.Parceiro, UserTypes.Admin })
        {
            if (!await roleManager.RoleExistsAsync(r))
            {
                await roleManager.CreateAsync(new ApplicationRole { Id = Guid.NewGuid(), Name = r, NormalizedName = r.ToUpperInvariant()});
            }
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            FirstName = "Nome",
            LastName = "sobrenome"
        };
        (await userManager.CreateAsync(user, "abc123")).Succeeded.ShouldBeTrue();

        // Sempre adiciona Default
        if (!await userManager.IsInRoleAsync(user, UserTypes.Default))
        {
            (await userManager.AddToRoleAsync(user, UserTypes.Default)).Succeeded.ShouldBeTrue();
        }

        foreach (var r in roles.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!await userManager.IsInRoleAsync(user, r))
            {
                (await userManager.AddToRoleAsync(user, r)).Succeeded.ShouldBeTrue();
            }
        }

        return user;
    }

    private static ClaimsPrincipal BuildPrincipal(Guid userId, params string[] roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        foreach (var r in roles.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            claims.Add(new Claim(ClaimTypes.Role, r));
        }

        // Emite claims de permissão baseados nos roles (cookbook)
        var roleToPermissions = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            [UserTypes.Default] = new[] { "users:access" },
            [UserTypes.Autor] = new[] { "autor:access" },
            [UserTypes.Parceiro] = new[] { "parceiro:access" },
            [UserTypes.Admin] = new[] { "users:admin" } // handler libera tudo a partir desta
        };

        foreach (var r in roles.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (roleToPermissions.TryGetValue(r, out var perms))
            {
                foreach (var p in perms)
                {
                    claims.Add(new Claim("permission", p));
                }
            }
        }

        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public async Task Default_User_Has_UsersAccess_Only()
    {
        using var sp = BuildAuthProvider();
        var user = await CreateUserWithRolesAsync(sp, "default@test.com");
        var principal = BuildPrincipal(user.Id, UserTypes.Default);

        var authz = sp.GetRequiredService<IAuthorizationService>();
        var policyProvider = sp.GetRequiredService<IAuthorizationPolicyProvider>();

        var usersAccess = await policyProvider.GetPolicyAsync("users:access");
        var parceiroAccess = await policyProvider.GetPolicyAsync("parceiro:access");

        (await authz.AuthorizeAsync(principal, null, usersAccess!)).Succeeded.ShouldBeTrue();
        (await authz.AuthorizeAsync(principal, null, parceiroAccess!)).Succeeded.ShouldBeFalse();
    }

    [Fact]
    public async Task Autor_Has_AutorAccess_But_Not_ParceiroAccess()
    {
        using var sp = BuildAuthProvider();
        var user = await CreateUserWithRolesAsync(sp, "autor@test.com", UserTypes.Autor);
        var principal = BuildPrincipal(user.Id, UserTypes.Default, UserTypes.Autor);

        var authz = sp.GetRequiredService<IAuthorizationService>();
        var policyProvider = sp.GetRequiredService<IAuthorizationPolicyProvider>();

        var autorAccess = await policyProvider.GetPolicyAsync("autor:access");
        var parceiroAccess = await policyProvider.GetPolicyAsync("parceiro:access");

        (await authz.AuthorizeAsync(principal, null, autorAccess!)).Succeeded.ShouldBeTrue();
        (await authz.AuthorizeAsync(principal, null, parceiroAccess!)).Succeeded.ShouldBeFalse();
    }

    [Fact]
    public async Task Parceiro_Has_ParceiroAccess_But_Not_AutorAccess()
    {
        using var sp = BuildAuthProvider();
        var user = await CreateUserWithRolesAsync(sp, "parceiro@test.com", UserTypes.Parceiro);
        var principal = BuildPrincipal(user.Id, UserTypes.Default, UserTypes.Parceiro);

        var authz = sp.GetRequiredService<IAuthorizationService>();
        var policyProvider = sp.GetRequiredService<IAuthorizationPolicyProvider>();

        var autorAccess = await policyProvider.GetPolicyAsync("autor:access");
        var parceiroAccess = await policyProvider.GetPolicyAsync("parceiro:access");
        var usersAccess = await policyProvider.GetPolicyAsync("users:access");

        (await authz.AuthorizeAsync(principal, null, usersAccess!)).Succeeded.ShouldBeTrue();
        (await authz.AuthorizeAsync(principal, null, parceiroAccess!)).Succeeded.ShouldBeTrue();
        (await authz.AuthorizeAsync(principal, null, autorAccess!)).Succeeded.ShouldBeFalse();
    }

    [Fact]
    public async Task Admin_Has_All_Permissions()
    {
        using var sp = BuildAuthProvider();
        var user = await CreateUserWithRolesAsync(sp, "admin@test.com", UserTypes.Admin);
        var principal = BuildPrincipal(user.Id, UserTypes.Default, UserTypes.Admin);

        var authz = sp.GetRequiredService<IAuthorizationService>();
        var policyProvider = sp.GetRequiredService<IAuthorizationPolicyProvider>();

        foreach (var policyName in new[] { "users:access", "users:admin", "autor:access", "parceiro:access", "qualquer:coisa" })
        {
            var policy = await policyProvider.GetPolicyAsync(policyName);
            (await authz.AuthorizeAsync(principal, null, policy!)).Succeeded.ShouldBeTrue($"Admin should pass policy '{policyName}'");
        }
    }

    [Fact]
    public async Task Parceiro_Does_Not_Have_TerceiroAccess()
    {
        using var sp = BuildAuthProvider();
        var user = await CreateUserWithRolesAsync(sp, "parceiro2@test.com", UserTypes.Parceiro);
        var principal = BuildPrincipal(user.Id, UserTypes.Default, UserTypes.Parceiro);

        var authz = sp.GetRequiredService<IAuthorizationService>();
        var policyProvider = sp.GetRequiredService<IAuthorizationPolicyProvider>();

        // Mesmo que a policy seja criada dinamicamente, o handler só autoriza se a permissão existir para o usuário.
        var terceiroAccess = await policyProvider.GetPolicyAsync("terceiro:access");
        (await authz.AuthorizeAsync(principal, null, terceiroAccess!)).Succeeded.ShouldBeFalse();
    }
}
