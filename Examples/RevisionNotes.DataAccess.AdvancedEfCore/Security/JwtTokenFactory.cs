using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RevisionNotes.DataAccess.AdvancedEfCore.Security;

public sealed class JwtTokenFactory
{
    public string CreateToken(JwtIssuerOptions options, string username)
    {
        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(ClaimTypes.Name, username)
            ],
            notBefore: now,
            expires: now.AddMinutes(options.ExpiresMinutes),
            signingCredentials: new SigningCredentials(CreateSigningKey(options.SigningKey), SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static SymmetricSecurityKey CreateSigningKey(string signingKey) =>
        new(Encoding.UTF8.GetBytes(signingKey));
}
