using System.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace Nastice.GoogleAuthenticateLab.Shared.Enums;

public enum LoginResultCode
{
    [Description("登入 OK")]
    Success,

    [Description("密碼錯誤")]
    PasswordInvalid,

    [Description("請輸入 Verification Code")]
    RequiredTotp
}
