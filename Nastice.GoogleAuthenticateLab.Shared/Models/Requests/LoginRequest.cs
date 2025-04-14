using System.ComponentModel.DataAnnotations;

namespace Nastice.GoogleAuthenticateLab.Shared.Models.Requests;

public record LoginRequest
{
    /// <summary>
    /// 帳號
    /// </summary>
    [Display(Name = "帳號")]
    public string? Account { get; set; }

    /// <summary>
    /// 密碼
    /// </summary>
    [Display(Name = "密碼")]
    public string? Password { get; set; }

    /// <summary>
    ///
    /// </summary>
    [Display(Name = "授權碼")]
    public string? Otp { get; set; }
}