using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Demesne.Server.Auth;

public class TokenService
{
    private readonly SymmetricSecurityKey _key;
    private readonly string _issuer;
    private readonly string _audience;

    public TokenService(IConfiguration config)
    {
        var jwt = config.GetSection("Jwt");
        var secret = jwt["Secret"]
            ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        _issuer = jwt["Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
        _audience = jwt["Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience is not configured.");
    }

    public string GenerateToken(string playerId, string username)
    {
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, playerId),
            new Claim("preferred_username", username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
