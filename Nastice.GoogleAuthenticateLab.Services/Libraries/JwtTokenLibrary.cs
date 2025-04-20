using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nastice.GoogleAuthenticateLab.Shared.Models.Options;

namespace Nastice.GoogleAuthenticateLab.Services.Libraries;

public class JwtTokenLibrary
{
    private readonly JwtOptions _jwtOptions;

    public JwtTokenLibrary(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public JwtOptions GetJwtOptions() => _jwtOptions;

    public string GenerateJwtToken(IEnumerable<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret!));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.DefaultAudience,
            claims,
            expires: DateTime.Now.AddMinutes(_jwtOptions.TokenLifetime),
            signingCredentials: signingCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenString;
    }
}
