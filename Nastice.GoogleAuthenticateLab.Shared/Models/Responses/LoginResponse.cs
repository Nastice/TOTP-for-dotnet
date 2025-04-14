namespace Nastice.GoogleAuthenticateLab.Shared.Models.Responses;

public record LoginResponse
{
    /// <summary>
    /// 令牌類型
    /// </summary>
    public string? TokenType { get; set; }

    /// <summary>
    /// 登入令牌
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// 有效時間
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// 過期時間
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}
