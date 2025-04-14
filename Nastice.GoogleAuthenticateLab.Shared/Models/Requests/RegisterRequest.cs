using System.ComponentModel.DataAnnotations;

namespace Nastice.GoogleAuthenticateLab.Shared.Models.Requests;

public record RegisterRequest
{
    [Display(Name = "帳號")]
    public string? Account { get; set; }

    [Display(Name = "密碼")]
    public string? Password { get; set; }

    [Display(Name = "密碼確認")]
    public string? PasswordConfirm { get; set; }

    [Display(Name = "名字")]
    public string? Name { get; set; }

    [Display(Name = "信箱")]
    public string? Email { get; set; }
}
