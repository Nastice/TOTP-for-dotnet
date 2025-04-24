using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Nastice.GoogleAuthenticateLab.Services.Interfaces;
using Nastice.GoogleAuthenticateLab.Shared;
using Nastice.GoogleAuthenticateLab.Shared.Models;
using Nastice.GoogleAuthenticateLab.Shared.Models.Requests;
using Nastice.GoogleAuthenticateLab.Shared.Models.Responses;

namespace Nastice.GoogleAuthenticateLab.Controllers;

[Route("api/[controller]")]
[ApiController]
[Tags("登入註冊等驗證相關功能")]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;
    private readonly IHostEnvironment _env;

    public AuthController(IAuthService authService, IHostEnvironment env)
    {
        _authService = authService;
        _env = env;
    }

    /// <summary>
    /// 一般登入
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("Login", Name = "Login")]
    [EndpointSummary("登入")]
    [EndpointDescription("依據帳號密碼進行登入，若使用者有申請 TOTP 功能則會要求提供 OTP 驗證碼")]
    [ProducesResponseType(typeof(ClientUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var loginResult = await _authService.TryLoginAsync(request);

        setTokenCookie(loginResult);

        return Ok(loginResult.User);
    }

    /// <summary>
    /// 註冊帳號
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("Register", Name="Register")]
    [EndpointSummary("註冊帳號")]
    [EndpointDescription("依據使用者的註冊資料寫入資料庫中。")]
    [ProducesResponseType(typeof(ClientUserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var registerResult = await _authService.RegisterAsync(request);

        setTokenCookie(registerResult);

        return Ok(registerResult.User);
    }

    /// <summary>
    /// 取得個人資料
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPost("Me", Name = "Get Profile")]
    [RequireCsrfToken]
    [EndpointSummary("取得個人資料")]
    [EndpointDescription("透過 Cookie 中的 access_token，取得使用者個人資料。")]
    [ProducesResponseType(typeof(ClientUserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Me()
    {
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(name))
        {
            var problemDetail = createProblemDetails(StatusCodes.Status401Unauthorized, "登入已失效，請重新登入");
            return Unauthorized(problemDetail);
        }

        var user = await _authService.GetUserByAccountAsync(name);
        if (user is null)
        {
            var problemDetail = createProblemDetails(StatusCodes.Status401Unauthorized, "登入已失效，請重新登入");
            return Unauthorized(problemDetail);
        }

        var response = new ClientUserResponse
        {
            Name = user.Name,
            Avatar = "",
            Email = user.Email
        };

        return Ok(response);
    }

    #region Private Methods

    private void setTokenCookie(LoginResult loginInfo)
    {
        setJwtTokenCookie(loginInfo);
        setRefreshTokenCookie(loginInfo);
        setCsrfTokenCookie(loginInfo);
    }

    /// <summary>
    /// 設定 Jwt Token Cookie
    /// </summary>
    /// <param name="loginInfo"></param>
    private void setJwtTokenCookie(LoginResult loginInfo)
    {
        var jwtOptions = createCookieOptions(loginInfo.ExpiresIn);

        Response.Cookies.Append("access_token", loginInfo.AccessToken, jwtOptions);
    }

    /// <summary>
    /// 設定更新 Token Cookie
    /// </summary>
    /// <param name="loginInfo"></param>
    private void setRefreshTokenCookie(LoginResult loginInfo)
    {
        var jwtOptions = createCookieOptions(loginInfo.RefreshTokenLifetime);

        Response.Cookies.Append("refresh_token", loginInfo.RefreshToken, jwtOptions);
    }

    /// <summary>
    /// 設定 CSRF Token Cookie
    /// </summary>
    /// <param name="loginInfo"></param>
    private void setCsrfTokenCookie(LoginResult loginInfo)
    {
        var csrfOptions = createCookieOptions(loginInfo.ExpiresIn, false);

        Response.Cookies.Append("csrf_token", loginInfo.CsrfToken, csrfOptions);
    }

    private CookieOptions createCookieOptions(int expiresIn, bool httpOnly = true)
    {
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddMinutes(expiresIn),
            Secure = true,
            SameSite = _env.IsDevelopment() ? SameSiteMode.None : SameSiteMode.Lax,
            HttpOnly = httpOnly
        };

        if (!_env.IsDevelopment())
        {
            cookieOptions.Domain = ".nastice.dev";
        }

        return cookieOptions;
    }

    #endregion
}
