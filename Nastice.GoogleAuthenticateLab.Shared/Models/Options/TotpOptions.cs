namespace Nastice.GoogleAuthenticateLab.Shared.Models.Options;

public class TotpOptions
{
    /// <summary>
    /// 發行者
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// 密碼長度
    /// </summary>
    public int CodeDigits { get; set; }

    /// <summary>
    /// 密碼更新時間
    /// </summary>
    public int Period { get; set; }
}
