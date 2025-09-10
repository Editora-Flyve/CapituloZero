using System.Security.Claims;
using System.Text.Json;
using CapituloZero.Infra.IdentityApp;
using CapituloZero.WebApp.Components.Account.Pages;
using CapituloZero.WebApp.Components.Account.Pages.Manage;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using CapituloZero.WebApp.Client.Models;

namespace CapituloZero.WebApp.Components.Account
{
    internal static class IdentityComponentsEndpointRouteBuilderExtensions
    {
        // These endpoints are required by the Identity Razor components defined in the /Components/Account/Pages directory of this project.
        public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
        {
            ArgumentNullException.ThrowIfNull(endpoints);
            
            endpoints.MapGet("/antiforgery", (IAntiforgery af, HttpContext ctx) =>
            {
                var tokens = af.GetAndStoreTokens(ctx);
                return Results.Ok(new { token = tokens.RequestToken });
            });
            
            var accountGroup = endpoints.MapGroup("/Account");

            accountGroup.MapGet("/UserRoles", async (
                HttpContext context,
                [FromServices] UserManager<ApplicationUser> userManager
                ) =>
            {
                if(context.User.Identity is null || !context.User.Identity.IsAuthenticated)
                    return Results.Ok(Array.Empty<string>());
                
                var user = await userManager.GetUserAsync(context.User);
                if (user is null)
                    return Results.Ok(Array.Empty<string>());
                var roles = await userManager.GetRolesAsync(user);
                return Results.Ok(roles);
            });
            
            accountGroup.MapPost("/Logout", async (
                ClaimsPrincipal user,
                [FromServices] SignInManager<ApplicationUser> signInManager,
                [FromForm] string returnUrl) =>
            {
                await signInManager.SignOutAsync();
                return TypedResults.LocalRedirect($"~/{returnUrl}");
            });

            // Simple JSON login endpoint for WASM client component.
            accountGroup.MapPost("/LoginApi", async (
                [FromServices] SignInManager<ApplicationUser> signInManager,
                [FromServices] ILoggerFactory loggerFactory,
                HttpContext context,
                [FromBody] LoginRequest request) =>
            {
                // Opcional: validar antiforgery se header presente
                if (context.Request.Headers.TryGetValue("RequestVerificationToken", out var _))
                {
                    var af = context.RequestServices.GetRequiredService<IAntiforgery>();
                    await af.ValidateRequestAsync(context);
                }
                var logger = loggerFactory.CreateLogger("LoginApi");
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
                {
                    return Results.BadRequest(new { message = "Credenciais inválidas." });
                }

                var result = await signInManager.PasswordSignInAsync(request.Email, request.Senha, request.Lembrar, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    logger.LogInformation("User {Email} logged in via LoginApi", request.Email);
                    return Results.Ok(new { success = true });
                }
                if (result.RequiresTwoFactor)
                {
                    return Results.Ok(new { success = false, requiresTwoFactor = true });
                }
                if (result.IsLockedOut)
                {
                    return Results.Ok(new { success = false, lockedOut = true });
                }

                return Results.Ok(new { success = false, message = "Tentativa de login inválida." });
            });

            accountGroup.MapPost("/RegisterApi", async (
                [FromServices] UserManager<ApplicationUser> userManager,
                [FromServices] SignInManager<ApplicationUser> signInManager,
                [FromServices] ILoggerFactory loggerFactory,
                HttpContext context,
                [FromBody] RegistrarRequest request) =>
            {
                if (context.Request.Headers.TryGetValue("RequestVerificationToken", out var _))
                {
                    var af = context.RequestServices.GetRequiredService<IAntiforgery>();
                    await af.ValidateRequestAsync(context);
                }
                var logger = loggerFactory.CreateLogger("RegisterApi");
                if (string.IsNullOrWhiteSpace(request.Nome) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
                {
                    return Results.BadRequest(new { message = "Campos obrigatórios ausentes." });
                }

                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email
                };
                var result = await userManager.CreateAsync(user, request.Senha);
                if (result.Succeeded)
                {
                    logger.LogInformation("User {Email} registered via RegisterApi", request.Email);
                    var requiresConfirmation = userManager.Options.SignIn.RequireConfirmedAccount;
                    if (!requiresConfirmation)
                    {
                        await signInManager.SignInAsync(user, isPersistent: false);
                    }
                    return Results.Ok(new { success = true, requiresConfirmation });
                }
                return Results.Ok(new { success = false, errors = result.Errors.Select(e => e.Description).ToArray() });
            });

            var manageGroup = accountGroup.MapGroup("/Manage").RequireAuthorization();

            var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var downloadLogger = loggerFactory.CreateLogger("DownloadPersonalData");

            manageGroup.MapPost("/DownloadPersonalData", async (
                HttpContext context,
                [FromServices] UserManager<ApplicationUser> userManager,
                [FromServices] AuthenticationStateProvider authenticationStateProvider) =>
            {
                var user = await userManager.GetUserAsync(context.User);
                if (user is null)
                {
                    return Results.NotFound($"Unable to load user with ID '{userManager.GetUserId(context.User)}'.");
                }

                var userId = await userManager.GetUserIdAsync(user);
                downloadLogger.LogInformation("User with ID '{UserId}' asked for their personal data.", userId);

                // Only include personal data for download
                var personalData = new Dictionary<string, string>();
                var personalDataProps = typeof(ApplicationUser).GetProperties()
                    .Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
                foreach (var p in personalDataProps)
                {
                    personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
                }

                var logins = await userManager.GetLoginsAsync(user);
                foreach (var l in logins)
                {
                    personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
                }

                personalData.Add("Authenticator Key", (await userManager.GetAuthenticatorKeyAsync(user))!);
                var fileBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

                context.Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
                return TypedResults.File(fileBytes, contentType: "application/json", fileDownloadName: "PersonalData.json");
            });

            return accountGroup;
        }
    }
}