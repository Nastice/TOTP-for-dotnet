using Nastice.GoogleAuthenticateLab.Shared.Models.Responses;

namespace Nastice.GoogleAuthenticateLab.Shared.Models;

public class LoginResult
{
    public required string AccessToken { get; set; }

    public required string CsrfToken { get; set; }

    public required string RefreshToken { get; set; }

    public int RefreshTokenLifetime { get; set; }

    public int ExpiresIn { get; set; }

    public DateTime ExpiredAt { get; set; }

    public ClientUserResponse? User { get; set; }
}
