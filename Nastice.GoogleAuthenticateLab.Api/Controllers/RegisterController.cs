using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nastice.GoogleAuthenticateLab.Services.Interfaces;
using Nastice.GoogleAuthenticateLab.Shared.Models.Requests;
using Nastice.GoogleAuthenticateLab.Shared.Models.Responses;
using Nastice.GoogleAuthenticateLab.Shared.Resources;

namespace Nastice.GoogleAuthenticateLab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly IAuthService _authService;

        public RegisterController(ILogger<RegisterController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        /// <summary>
        /// 註冊使用者
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var user = await _authService.RegisterAsync(request);
            if (user is null)
            {
                _logger.LogError(LogMessages.Api.Controllers.RegisterController.UserCreatedFailed, request.Account);
                return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "帳號無法註冊！請稍後再試。");
            }

            var loginResponse = _authService.CreateToken(user);

            return Ok(loginResponse);
        }
    }
}
