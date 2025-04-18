using Microsoft.AspNetCore.Mvc;
using Nastice.GoogleAuthenticateLab.Services.Interfaces;
using Nastice.GoogleAuthenticateLab.Shared.Enums;
using Nastice.GoogleAuthenticateLab.Shared.Extensions;
using Nastice.GoogleAuthenticateLab.Shared.Models.Requests;
using Nastice.GoogleAuthenticateLab.Shared.Resources;

namespace Nastice.GoogleAuthenticateLab.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;
    private readonly IAuthService _authService;
    private readonly IHostEnvironment _env;

    public LoginController(ILogger<LoginController> logger, IAuthService authService, IHostEnvironment env)
    {
        _logger = logger;
        _authService = authService;
        _env = env;
    }

    /// <summary>
    /// 一般登入
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _authService.GetUserByAccountAsync(request.Account!);
        if (user == null)
        {
            _logger.LogError(
                LogMessages.Api.Controllers.LoginController.UserNotFound,
                request.Account
            );

            var problemDetail = createUnauthorizedProblemDetails(
                LoginResultCode.InvalidAccountOrPassword.GetDisplayName()
            );
            return Unauthorized(problemDetail);
        }

        var isAuthorize = _authService.TryLogin(user, request.Password!, request.Otp!);
        if (isAuthorize is not LoginResultCode.Success)
        {
            _logger.LogError(
                LogMessages.Api.Controllers.LoginController.Unauthorized,
                request.Account,
                isAuthorize.ToString()
            );

            var problemDetail = createUnauthorizedProblemDetails(
                isAuthorize.GetDisplayName(),
                isAuthorize
            );

            return Unauthorized(problemDetail);
        }

        var loginResponse = _authService.CreateToken(user);

        setJwtTokenCookie(loginResponse.AccessToken!);
        setCsrfTokenCookie();

        return Ok(loginResponse);
    }

    private ProblemDetails createUnauthorizedProblemDetails(
        string title,
        LoginResultCode code = LoginResultCode.InvalidAccountOrPassword
    )
    {
        var problemDetails = ProblemDetailsFactory.CreateProblemDetails(
            HttpContext,
            StatusCodes.Status401Unauthorized,
            title
        );

        problemDetails.Extensions.Add("errorCode", code.ToString());

        return problemDetails;
    }

    private void setJwtTokenCookie(string accessToken)
    {
        var jwtOptions = new CookieOptions
        {
            Domain = _env.IsDevelopment() ? "" : ".nastice.dev",
            Expires = DateTime.Now.AddHours(1),
            Secure = true,
            SameSite = SameSiteMode.None,
            HttpOnly = true
        };

        Response.Cookies.Append("access_token", accessToken, jwtOptions);
    }

    private void setCsrfTokenCookie()
    {
        var csrfOptions = new CookieOptions
        {
            Domain = _env.IsDevelopment() ? "" : ".nastice.dev",
            Expires = DateTimeOffset.Now.AddHours(1),
            Secure = true,
            SameSite = SameSiteMode.None,
            HttpOnly = false
        };

        Response.Cookies.Append("csrf_token", Guid.NewGuid().ToString(), csrfOptions);
    }
}
