using System.ComponentModel.DataAnnotations;

namespace Nastice.GoogleAuthenticateLab.Shared.Enums;

public enum LoginResultCode
{
    /// <summary>
    /// OK
    /// </summary>
    Success,

    /// <summary>
    /// 帳號密碼錯誤
    /// </summary>
    [Display(Name = "帳號或密碼輸入錯誤")]
    InvalidAccountOrPassword,

    /// <summary>
    /// 授權碼為空
    /// </summary>
    [Display(Name = "請輸入您的授權碼")]
    OtpIsEmpty,

    /// <summary>
    /// 授權碼錯誤
    /// </summary>
    [Display(Name = "授權碼錯誤")]
    InvalidOtp,

    /// <summary>
    /// 帳號目前禁止登入
    /// </summary>
    [Display(Name = "您的帳號目前無法登入")]
    NotEnable
}
