namespace Nastice.GoogleAuthenticateLab.Shared.Models.Responses;

public class LoginResponse
{
    /// <summary>
    /// 令牌類型
    /// </summary>
    public string? TokenType { get; set; }

    /// <summary>
    /// 登入令牌
    /// </summary>
    public string? AccessToken { get; set; }
}
