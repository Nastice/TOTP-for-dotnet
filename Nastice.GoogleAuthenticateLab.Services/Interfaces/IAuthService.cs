using Nastice.GoogleAuthenticateLab.Data.Nastice.GoogleAuthenticateLab.Data.DTOs;

namespace Nastice.GoogleAuthenticateLab.Services.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// 透過帳號取得使用者
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public Task<User?> GetUserByAccountAsync(string account);
}
