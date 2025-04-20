using System.ComponentModel;

namespace Nastice.GoogleAuthenticateLab.Shared.Models.Requests;

public record RegisterRequest
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
    /// 密碼確認
    /// </summary>
    [Description("密碼確認")]
    public required string PasswordConfirm { get; set; }

    /// <summary>
    /// 顯示名稱，
    /// </summary>
    [Description("姓名")]
    public required string Name { get; set; }

    /// <summary>
    /// 信箱
    /// </summary>
    [Description("信箱")]
    public required string Email { get; set; }
}
