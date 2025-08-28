using System.Security.Claims;
using System.Text;
using CapituloZero.Application.Abstractions.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CapituloZero.Infrastructure.Authentication;
#pragma warning disable CA1812 // Avoid uninstantiated internal classes - instantiated by DI container

internal sealed class TokenProvider(IConfiguration configuration) : ITokenProvider
{
    public string Create(Guid userId, string email, IEnumerable<string> roles)
    {
        string secretKey = configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Include basic claims and provided role claims
        List<Claim> claims = new()
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            // Garante compatibilidade com ClaimTypes.NameIdentifier
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };

        var roleList = roles?.ToList() ?? [];
        foreach (var role in roleList)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        if (roleList.Count > 0)
        {
            claims.Add(new Claim("cz:user_types", string.Join(' ', roleList)));
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
