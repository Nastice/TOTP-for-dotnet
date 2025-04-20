using System.ComponentModel;

namespace Nastice.GoogleAuthenticateLab.Shared.Models.Responses;

public class ClientUserResponse
{
    /// <summary>
    /// 使用者名稱
    /// </summary>
    [Description("使用者名稱")]
    public string? Name { get; set; }

    /// <summary>
    /// 大頭貼
    /// </summary>
    [Description("大頭貼網址")]
    public string? Avatar { get; set; }

    /// <summary>
    /// 信箱
    /// </summary>
    [Description("使用者信箱")]
    public string? Email { get; set; }
}
