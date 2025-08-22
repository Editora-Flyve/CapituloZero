using System.Security.Claims;
using System.Text;
using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Domain.Users;
using CapituloZero.Infrastructure.Usuarios;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CapituloZero.Infrastructure.Authentication;
#pragma warning disable CA1812 // Avoid uninstantiated internal classes - instantiated by DI container

internal sealed class TokenProvider(IConfiguration configuration, UserManager<ApplicationUser> userManager) : ITokenProvider
{
    public string Create(User user)
    {
        // Backwards compat mapping path not used by new Identity flow; keep for interface
        return CreateInternal(user.Id, user.Email);
    }

    private string CreateInternal(Guid userId, string email)
    {
        string secretKey = configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Try fetch roles if we have the ApplicationUser
        List<Claim> claims = new()
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email)
        };

        try
        {
            var appUser = userManager.Users.FirstOrDefault(u => u.Id == userId);
            if (appUser is not null)
            {
                var roles = userManager.GetRolesAsync(appUser).GetAwaiter().GetResult();
                if (roles is not null)
                {
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                    claims.Add(new Claim("cz:user_types", string.Join(' ', roles)));
                }
            }
        }
    catch (InvalidOperationException)
        {
            // ignore role enrichment failures
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
            SigningCredentials = credentials,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"]
        };

        var handler = new JsonWebTokenHandler();
        return handler.CreateToken(tokenDescriptor);
    }
}
#pragma warning restore CA1812
