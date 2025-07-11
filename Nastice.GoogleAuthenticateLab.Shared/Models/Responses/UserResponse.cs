using System.ComponentModel;

namespace Nastice.GoogleAuthenticateLab.Shared.Models.Responses;

public class UserResponse
{
    [Description("流水號")]
    public int Id { get; set; }

    [Description("帳號")]
    public required string Account { get; set; }

    [Description("名字")]
    public required string Name { get; set; }

    [Description("信箱")]
    public required string Email { get; set; }

    [Description("是否啟用二階段驗證")]
    public bool TotpEnabled { get; set; }

    [Description("最後登入時間")]
    public DateTime LastLogonAt { get; set; }

    [Description("註冊時間")]
    public DateTime RegisteredAt { get; set; }
}
