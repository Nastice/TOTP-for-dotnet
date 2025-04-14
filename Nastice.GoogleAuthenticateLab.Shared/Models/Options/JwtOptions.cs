namespace Nastice.GoogleAuthenticateLab.Shared.Models.Options;

public record JwtOptions
{
    /// <summary>
    /// 密鑰
    /// </summary>
    public string? Secret { get; set; }

    /// <summary>
    /// 發行者
    /// </summary>
    public string? Issuer { get; set; }

    /// <summary>
    /// 預設接收者
    /// </summary>
    public string? DefaultAudience { get; set; }

    /// <summary>
    /// Token 有效時間
    /// </summary>
    public int TokenLifetime { get; set; }

    /// <summary>
    /// 替換用 Token 有效時間
    /// </summary>
    public int RefreshTokenLifetime { get; set; }
};
