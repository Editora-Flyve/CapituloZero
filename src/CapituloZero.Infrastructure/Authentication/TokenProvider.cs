using System.Security.Claims;
using System.Text;
using CapituloZero.Application.Abstractions.Authentication;
using CapituloZero.Domain.Users.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using CapituloZero.Domain.Users;

namespace CapituloZero.Infrastructure.Authentication;

internal sealed class TokenProvider(IConfiguration configuration) : ITokenProvider
{
    public string Create(User user)
    {
        string secretKey = configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        // Add active type claim if present
        if (user.ActiveType is UserType active && active != UserType.None)
        {
            claims.Add(new Claim("active_type", active.ToString()));
        }

        // Add role claims for all types for convenience-based authorization
        foreach (UserType t in Enum.GetValues<UserType>())
        {
            if (t == UserType.None) continue;
            if (user.Types.HasFlag(t))
            {
                claims.Add(new Claim(ClaimTypes.Role, t.ToString()));
            }
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

        string token = handler.CreateToken(tokenDescriptor);

        return token;
    }
}
