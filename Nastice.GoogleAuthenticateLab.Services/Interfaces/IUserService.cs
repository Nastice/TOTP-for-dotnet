using System.Security.Claims;
using Nastice.GoogleAuthenticateLab.Data.Nastice.GoogleAuthenticateLab.Data.DTOs;

namespace Nastice.GoogleAuthenticateLab.Services.Interfaces;

public interface IUserService
{
    /// <summary>
    /// 透過登入資訊取得使用者
    /// </summary>
    /// <param name="userClaims"></param>
    /// <returns></returns>
    public Task<User?> GetUserByClaims(ClaimsPrincipal userClaims);
}
