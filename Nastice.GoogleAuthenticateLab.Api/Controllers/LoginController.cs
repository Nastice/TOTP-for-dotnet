using Microsoft.AspNetCore.Mvc;
using Nastice.GoogleAuthenticateLab.Shared.Models.Requests;

namespace Nastice.GoogleAuthenticateLab.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    /// <summary>
    /// 一般登入
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        await Task.CompletedTask;

        return Ok();
    }
}