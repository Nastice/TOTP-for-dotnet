using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nastice.GoogleAuthenticateLab.Shared.Models.Options;
using Nastice.GoogleAuthenticateLab.Shared.Models.Responses;

namespace Nastice.GoogleAuthenticateLab.Services.Libraries;

public class JwtTokenLibrary
{
    private readonly JwtOptions _jwtOptions;

    public JwtTokenLibrary(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public LoginResponse CreateLoginResponse(IEnumerable<Claim> claims)
    {
        var response = new LoginResponse();

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

        response.AccessToken = tokenString;
        response.ExpiresIn = _jwtOptions.TokenLifetime;
        response.TokenType = "Bearer";
        response.ExpiresAt = DateTime.Now.AddMinutes(_jwtOptions.TokenLifetime);

        return response;
    }
}
