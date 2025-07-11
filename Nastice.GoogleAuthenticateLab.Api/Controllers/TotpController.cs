using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nastice.GoogleAuthenticateLab.Services.Interfaces;
using Nastice.GoogleAuthenticateLab.Shared.Extensions;
using Nastice.GoogleAuthenticateLab.Shared.Helpers;
using Nastice.GoogleAuthenticateLab.Shared.Models.Responses;

namespace Nastice.GoogleAuthenticateLab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TotpController : ApiControllerBase
    {
        private readonly ITotpService _totpService;
        private readonly IUserService _userService;

        public TotpController(ITotpService totpService, IUserService userService)
        {
            _totpService = totpService;
            _userService = userService;
        }

        [Authorize]
        [HttpGet("Totp", Name = "GenerateTotpUri")]
        [ProducesResponseType(typeof(TotpQrCodeResponse), StatusCodes.Status200OK)]
        [EndpointSummary("生成 Totp 驗證回應")]
        public async Task<IActionResult> GenerateTotpUri()
        {
            var user = await _userService.GetUserByClaims(User);
            if (user is null)
            {
                var problem = createProblemDetails(StatusCodes.Status401Unauthorized, "您的登入已失效，請重新登入");
                return Unauthorized(problem);
            }

            var secret = TotpHelpers.GenerateTotpSecret(10);
            var uri = _totpService.GenerateTotpUri(user, secret);

            var response = new TotpQrCodeResponse
            {
                QrCode = uri.ToString().ToQrcode(),
                TotpToken = secret
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPost("EnableTotp", Name = "EnableTotp")]
        [EndpointSummary("啟用 Authenticator 功能")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> EnableTotp()
        {
            return NoContent();
        }
    }
}
