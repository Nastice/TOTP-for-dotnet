using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nastice.GoogleAuthenticateLab.Services.Interfaces;
using Nastice.GoogleAuthenticateLab.Shared.Models.Responses;

namespace Nastice.GoogleAuthenticateLab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ApiControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("Profile", Name = "GetMyProfile")]
        [EndpointSummary("取得個人資料")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Profile()
        {
            var user = await _userService.GetUserByClaims(User);
            if (user is null)
            {
                var problemDetail = createProblemDetails(StatusCodes.Status401Unauthorized, "您的登入資訊已失效，請重新登入");

                return Unauthorized(problemDetail);
            }

            var response = new UserResponse
            {
                Id = user.Id,
                Account = user.Account,
                Name = user.Name,
                Email = user.Email,
                TotpEnabled = user.Secret is not null,
                LastLogonAt = user.LastLogonAt ?? DateTime.Now,
                RegisteredAt = user.CreatedAt
            };

            return Ok(response);
        }
    }
}
