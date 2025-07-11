using System.ComponentModel;

namespace Nastice.GoogleAuthenticateLab.Shared.Models.Responses;

public class TotpQrCodeResponse
{
    [Description("TOTP 的 Token 轉換為 QR Code")]
    public required string QrCode { get; set; }

    [Description("手動輸入的 token")]
    public required string TotpToken { get; set; }
}
