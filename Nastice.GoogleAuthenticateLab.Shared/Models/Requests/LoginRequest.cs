using System.ComponentModel;

namespace Nastice.GoogleAuthenticateLab.Shared.Models.Requests;

public record LoginRequest
{
    /// <summary>
    /// 帳號
    /// </summary>
    [Description("帳號")]
    public required string Account { get; set; }

    /// <summary>
    /// 密碼
    /// </summary>
    [Description("密碼")]
    public required string Password { get; set; }

    /// <summary>
    /// 一次性授權碼
    /// </summary>
    [Description("OTP 驗證碼")]
    public string? Otp { get; set; }
}