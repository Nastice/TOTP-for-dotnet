using System.ComponentModel.DataAnnotations;

namespace Nastice.GoogleAuthenticateLab.Shared.Enums;

public enum CsrfResultCode
{
        [Display(Name = "成功")]
        Success = 0,

        [Display(Name = "Header X-CSRF-Token 未提供")]
        EmptyCsrfToken = 1,

        [Display(Name = "未提供 JwtToken")]
        CouldNotFoundUser = 2,

        [Display(Name = "找不到 CSRF Token 紀錄")]
        StoredCsrfTokenNotFound = 3,

        [Display(Name = "CSRF Token 不匹配")]
        CsrfNotMatch = 4
}
