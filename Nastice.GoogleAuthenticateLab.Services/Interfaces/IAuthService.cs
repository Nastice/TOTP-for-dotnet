using Nastice.GoogleAuthenticateLab.Data.Nastice.GoogleAuthenticateLab.Data.DTOs;
using Nastice.GoogleAuthenticateLab.Shared.Enums;
using Nastice.GoogleAuthenticateLab.Shared.Models.Requests;
using Nastice.GoogleAuthenticateLab.Shared.Models.Responses;

namespace Nastice.GoogleAuthenticateLab.Services.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// 透過帳號取得使用者
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public Task<User?> GetUserByAccountAsync(string account);

    /// <summary>
    /// 驗證是否可以登入
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <param name="otp"></param>
    /// <returns></returns>
    public LoginResultCode TryLogin(User user, string password, string? otp);

    /// <summary>
    /// 生成登入回應
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public LoginResponse CreateToken(User user);

    /// <summary>
    /// 註冊使用者
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public Task<User?> RegisterAsync(RegisterRequest request);
}
