using Nastice.GoogleAuthenticateLab.Data.Nastice.GoogleAuthenticateLab.Data.DTOs;
using OtpNet;

namespace Nastice.GoogleAuthenticateLab.Services.Interfaces;

public interface ITotpService
{
    /// <summary>
    /// 生成 TOTP URI
    /// </summary>
    /// <param name="user"></param>
    /// <param name="secret"></param>
    /// <returns></returns>
    public OtpUri GenerateTotpUri(User user, string secret);
}
