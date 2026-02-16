using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RevisionNotes.StandardApi.Security;

public sealed class JwtTokenFactory
{
    public string CreateToken(JwtIssuerOptions options, string username)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "ApiUser")
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(options.TokenLifetimeMinutes),
            Issuer = options.Issuer,
            Audience = options.Audience,
            SigningCredentials = new SigningCredentials(CreateSigningKey(options.SigningKey), SecurityAlgorithms.HmacSha256)
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(descriptor);
        return handler.WriteToken(token);
    }

    public static SymmetricSecurityKey CreateSigningKey(string signingKey)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
    }
}